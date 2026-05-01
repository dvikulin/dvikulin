using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Taskify.Api.Dtos;
using Taskify.Api.Hubs;
using Taskify.Api.Validation;
using Taskify.Database;
using Taskify.Database.Models;
using TaskModel = Taskify.Database.Models.Task;

namespace Taskify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly TaskifyDbContext context;
    private readonly IHubContext<NotificationsHub> notifications;

    public TasksController(TaskifyDbContext context, IHubContext<NotificationsHub> notifications)
    {
        this.context = context;
        this.notifications = notifications;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetTasks([FromQuery] int? projectId)
    {
        var query = TaskQuery();
        if (projectId is not null)
        {
            query = query.Where(task => task.ProjectId == projectId);
        }

        var tasks = await query
            .OrderBy(task => task.Status)
            .ThenBy(task => task.Title)
            .ToListAsync();

        return tasks.Select(task => task.ToDto()).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        var task = await TaskQuery().FirstOrDefaultAsync(task => task.Id == id);
        return task is null ? NotFound() : task.ToDto();
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskRequest request)
    {
        var validation = ValidateTask(request.Title, request.Description);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var project = await context.Projects
            .Include(item => item.Members)
            .FirstOrDefaultAsync(item => item.Id == request.ProjectId);
        if (project is null)
        {
            return NotFound("Project not found.");
        }

        if (!TaskifyValidation.TryParseStatus(request.Status ?? "To Do", out var status))
        {
            return BadRequest("Status must be To Do, In Progress, In Review, or Done.");
        }

        if (!TaskifyValidation.TryParsePriority(request.Priority, out var priority))
        {
            return BadRequest("Priority must be Low, Medium, High, or Urgent.");
        }

        if (request.AssigneeId is not null && !project.Members.Any(member => member.UserId == request.AssigneeId))
        {
            return BadRequest("Assignee must be a project team member.");
        }

        var task = new TaskModel
        {
            ProjectId = request.ProjectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Status = status,
            Priority = priority,
            AssigneeId = request.AssigneeId,
            DueDate = request.DueDate
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var created = await TaskQuery().FirstAsync(item => item.Id == task.Id);
        await Publish(project.Id, "TaskCreated", created.ToDto());

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
    {
        var validation = ValidateTask(request.Title, request.Description);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var task = await context.Tasks
            .Include(item => item.Project)
            .ThenInclude(project => project.Members)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (task is null)
        {
            return NotFound();
        }

        if (!TaskifyValidation.TryParseStatus(request.Status, out var status))
        {
            return BadRequest("Status must be To Do, In Progress, In Review, or Done.");
        }

        if (!TaskifyValidation.TryParsePriority(request.Priority, out var priority))
        {
            return BadRequest("Priority must be Low, Medium, High, or Urgent.");
        }

        if (request.AssigneeId is not null && !task.Project.Members.Any(member => member.UserId == request.AssigneeId))
        {
            return BadRequest("Assignee must be a project team member.");
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim() ?? string.Empty;
        task.Status = status;
        task.Priority = priority;
        task.AssigneeId = request.AssigneeId;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var updated = await TaskQuery().FirstAsync(item => item.Id == id);
        await Publish(updated.ProjectId, "TaskUpdated", updated.ToDto());

        return Ok(updated.ToDto());
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateTaskStatusRequest request)
    {
        if (!TaskifyValidation.TryParseStatus(request.Status, out var status))
        {
            return BadRequest("Status must be To Do, In Progress, In Review, or Done.");
        }

        var task = await context.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var updated = await TaskQuery().FirstAsync(item => item.Id == id);
        await Publish(updated.ProjectId, "TaskUpdated", updated.ToDto());

        return Ok(updated.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await context.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        var projectId = task.ProjectId;
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        await Publish(projectId, "TaskDeleted", new { taskId = id });
        return NoContent();
    }

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(int id, CreateCommentRequest request)
    {
        var validation = TaskifyValidation.ValidateLength(request.Content, "Comment", 1000, required: true);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var task = await context.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound("Task not found.");
        }

        var authorExists = await context.Users.AnyAsync(user => user.Id == request.AuthorId);
        if (!authorExists)
        {
            return BadRequest("Author must be one of the predefined users.");
        }

        var comment = new Comment
        {
            TaskId = id,
            AuthorId = request.AuthorId,
            Content = request.Content.Trim()
        };

        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        var created = await CommentQuery().FirstAsync(item => item.Id == comment.Id);
        await Publish(task.ProjectId, "CommentAdded", created.ToDto());

        return CreatedAtAction(nameof(GetTask), new { id }, created.ToDto());
    }

    [HttpPut("{id:int}/comments/{commentId:int}")]
    public async Task<IActionResult> UpdateComment(int id, int commentId, UpdateCommentRequest request)
    {
        var validation = TaskifyValidation.ValidateLength(request.Content, "Comment", 1000, required: true);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var comment = await context.Comments
            .Include(item => item.Task)
            .Include(item => item.Author)
            .FirstOrDefaultAsync(item => item.Id == commentId && item.TaskId == id);
        if (comment is null)
        {
            return NotFound();
        }

        if (comment.AuthorId != request.AuthorId)
        {
            return Forbid();
        }

        comment.Content = request.Content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        await Publish(comment.Task.ProjectId, "CommentUpdated", comment.ToDto());

        return Ok(comment.ToDto());
    }

    [HttpDelete("{id:int}/comments/{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int id, int commentId, [FromQuery] int authorId)
    {
        var comment = await context.Comments
            .Include(item => item.Task)
            .FirstOrDefaultAsync(item => item.Id == commentId && item.TaskId == id);
        if (comment is null)
        {
            return NotFound();
        }

        if (comment.AuthorId != authorId)
        {
            return Forbid();
        }

        var projectId = comment.Task.ProjectId;
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
        await Publish(projectId, "CommentDeleted", new { taskId = id, commentId });

        return NoContent();
    }

    private IQueryable<TaskModel> TaskQuery() =>
        context.Tasks
            .Include(task => task.Assignee)
            .Include(task => task.Comments)
            .ThenInclude(comment => comment.Author);

    private IQueryable<Comment> CommentQuery() =>
        context.Comments.Include(comment => comment.Author);

    private System.Threading.Tasks.Task Publish(int projectId, string eventName, object payload) =>
        notifications.Clients
            .Group(NotificationsHub.ProjectGroup(projectId.ToString()))
            .SendAsync(eventName, payload);

    private static string? ValidateTask(string title, string? description) =>
        TaskifyValidation.ValidateLength(title, "Task title", 300, required: true)
        ?? TaskifyValidation.ValidateLength(description, "Task description", 2000, required: false);
}
