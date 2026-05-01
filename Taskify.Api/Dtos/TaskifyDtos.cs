namespace Taskify.Api.Dtos;

public sealed record UserDto(int Id, string Name, string Email, string Role);

public sealed record ProjectMemberDto(int Id, string Name, string Email, string Role);

public sealed record ProjectDto(
    int Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    UserDto Owner,
    IReadOnlyList<ProjectMemberDto> TeamMembers);

public sealed record CommentDto(
    int Id,
    string Content,
    UserDto Author,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record TaskDto(
    int Id,
    int ProjectId,
    string Title,
    string Description,
    string Status,
    string Priority,
    UserDto? Assignee,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DueDate,
    IReadOnlyList<CommentDto> Comments);

public sealed record CreateProjectRequest(string Name, string? Description, int OwnerId);

public sealed record UpdateProjectRequest(string Name, string? Description);

public sealed record ProjectMemberRequest(int UserId);

public sealed record CreateTaskRequest(
    int ProjectId,
    string Title,
    string? Description,
    int? AssigneeId,
    string? Status,
    string? Priority,
    DateTime? DueDate);

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    int? AssigneeId,
    string Status,
    string? Priority,
    DateTime? DueDate);

public sealed record UpdateTaskStatusRequest(string Status);

public sealed record CreateCommentRequest(int AuthorId, string Content);

public sealed record UpdateCommentRequest(int AuthorId, string Content);
