using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskify.Api.Dtos;
using Taskify.Api.Hubs;
using Taskify.Api.Validation;
using Taskify.Database;
using Taskify.Database.Models;

namespace Taskify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProjectsController : ControllerBase
{
    private readonly TaskifyDbContext context;

    public ProjectsController(TaskifyDbContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetProjects()
    {
        var projects = await ProjectQuery()
            .OrderBy(project => project.Name)
            .ToListAsync();

        return projects.Select(project => project.ToDto()).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetProject(int id)
    {
        var project = await ProjectQuery()
            .FirstOrDefaultAsync(project => project.Id == id);

        return project is null ? NotFound() : project.ToDto();
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectRequest request)
    {
        var validation = ValidateProject(request.Name, request.Description);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var owner = await context.Users.FindAsync(request.OwnerId);
        if (owner is null)
        {
            return BadRequest("Owner must be one of the predefined users.");
        }

        var duplicate = await context.Projects.AnyAsync(project => project.Name == request.Name.Trim());
        if (duplicate)
        {
            return BadRequest("Project name must be unique.");
        }

        var project = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            OwnerId = request.OwnerId
        };

        project.Members.Add(new ProjectMember
        {
            UserId = request.OwnerId,
            Role = ProjectRole.Owner
        });

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var created = await ProjectQuery().FirstAsync(item => item.Id == project.Id);
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProject(int id, UpdateProjectRequest request)
    {
        var validation = ValidateProject(request.Name, request.Description);
        if (validation is not null)
        {
            return BadRequest(validation);
        }

        var project = await context.Projects.FindAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        var name = request.Name.Trim();
        var duplicate = await context.Projects.AnyAsync(item => item.Id != id && item.Name == name);
        if (duplicate)
        {
            return BadRequest("Project name must be unique.");
        }

        project.Name = name;
        project.Description = request.Description?.Trim() ?? string.Empty;
        project.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok((await ProjectQuery().FirstAsync(item => item.Id == id)).ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        context.Projects.Remove(project);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/members")]
    public async Task<IActionResult> AddMember(int id, ProjectMemberRequest request)
    {
        var projectExists = await context.Projects.AnyAsync(project => project.Id == id);
        if (!projectExists)
        {
            return NotFound("Project not found.");
        }

        var userExists = await context.Users.AnyAsync(user => user.Id == request.UserId);
        if (!userExists)
        {
            return BadRequest("User must be one of the predefined users.");
        }

        var exists = await context.ProjectMembers.AnyAsync(member =>
            member.ProjectId == id && member.UserId == request.UserId);
        if (exists)
        {
            return BadRequest("User is already a project member.");
        }

        context.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = id,
            UserId = request.UserId,
            Role = ProjectRole.Member
        });

        await context.SaveChangesAsync();
        return Ok((await ProjectQuery().FirstAsync(project => project.Id == id)).ToDto());
    }

    [HttpDelete("{id:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember(int id, int userId)
    {
        var member = await context.ProjectMembers.FindAsync(id, userId);
        if (member is null)
        {
            return NotFound();
        }

        if (member.Role == ProjectRole.Owner)
        {
            return BadRequest("Project owner cannot be removed.");
        }

        context.ProjectMembers.Remove(member);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<Project> ProjectQuery() =>
        context.Projects
            .Include(project => project.Owner)
            .Include(project => project.Members)
            .ThenInclude(member => member.User);

    private static string? ValidateProject(string name, string? description) =>
        TaskifyValidation.ValidateLength(name, "Project name", 200, required: true)
        ?? TaskifyValidation.ValidateLength(description, "Project description", 1000, required: false);
}
