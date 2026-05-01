using Taskify.Database.Models;
using TaskModel = Taskify.Database.Models.Task;
using TaskStatusModel = Taskify.Database.Models.TaskStatus;

namespace Taskify.Api.Dtos;

public static class TaskifyMapping
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role);

    public static ProjectMemberDto ToMemberDto(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role);

    public static ProjectDto ToDto(this Project project)
    {
        var members = project.Members
            .Select(member => member.User.ToMemberDto())
            .OrderBy(member => member.Name)
            .ToArray();

        return new ProjectDto(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt,
            project.UpdatedAt,
            project.Owner.ToDto(),
            members);
    }

    public static TaskDto ToDto(this TaskModel task)
    {
        var comments = task.Comments
            .OrderBy(comment => comment.CreatedAt)
            .Select(comment => comment.ToDto())
            .ToArray();

        return new TaskDto(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            ToStatusLabel(task.Status),
            task.Priority.ToString(),
            task.Assignee?.ToDto(),
            task.CreatedAt,
            task.UpdatedAt,
            task.DueDate,
            comments);
    }

    public static CommentDto ToDto(this Comment comment) =>
        new(comment.Id, comment.Content, comment.Author.ToDto(), comment.CreatedAt, comment.UpdatedAt);

    public static string ToStatusLabel(TaskStatusModel status) =>
        status switch
        {
            TaskStatusModel.Todo => "To Do",
            TaskStatusModel.InProgress => "In Progress",
            TaskStatusModel.InReview => "In Review",
            TaskStatusModel.Done => "Done",
            _ => status.ToString()
        };
}
