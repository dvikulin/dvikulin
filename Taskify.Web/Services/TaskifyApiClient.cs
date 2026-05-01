using System.Net.Http.Json;
using Taskify.Web.Models;

namespace Taskify.Web.Services;

public sealed class TaskifyApiClient
{
    private readonly HttpClient http;

    public TaskifyApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync() =>
        await http.GetFromJsonAsync<IReadOnlyList<UserDto>>("api/users") ?? [];

    public async Task<IReadOnlyList<ProjectDto>> GetProjectsAsync() =>
        await http.GetFromJsonAsync<IReadOnlyList<ProjectDto>>("api/projects") ?? [];

    public async Task<ProjectDto?> GetProjectAsync(int projectId) =>
        await http.GetFromJsonAsync<ProjectDto>($"api/projects/{projectId}");

    public async Task<ProjectDto?> CreateProjectAsync(CreateProjectRequest request)
    {
        var response = await http.PostAsJsonAsync("api/projects", request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProjectDto>();
    }

    public async Task<ProjectDto?> AddMemberAsync(int projectId, int userId)
    {
        var response = await http.PostAsJsonAsync($"api/projects/{projectId}/members", new ProjectMemberRequest(userId));
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<ProjectDto>();
    }

    public async Task RemoveMemberAsync(int projectId, int userId)
    {
        var response = await http.DeleteAsync($"api/projects/{projectId}/members/{userId}");
        await EnsureSuccessAsync(response);
    }

    public async Task<IReadOnlyList<TaskDto>> GetTasksAsync(int projectId) =>
        await http.GetFromJsonAsync<IReadOnlyList<TaskDto>>($"api/tasks?projectId={projectId}") ?? [];

    public async Task<TaskDto?> CreateTaskAsync(CreateTaskRequest request)
    {
        var response = await http.PostAsJsonAsync("api/tasks", request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TaskDto>();
    }

    public async Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/tasks/{taskId}", request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TaskDto>();
    }

    public async Task<TaskDto?> UpdateStatusAsync(int taskId, string status)
    {
        var response = await http.PutAsJsonAsync($"api/tasks/{taskId}/status", new UpdateTaskStatusRequest(status));
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TaskDto>();
    }

    public async Task<CommentDto?> AddCommentAsync(int taskId, int authorId, string content)
    {
        var response = await http.PostAsJsonAsync($"api/tasks/{taskId}/comments", new CreateCommentRequest(authorId, content));
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CommentDto>();
    }

    public async Task<CommentDto?> UpdateCommentAsync(int taskId, int commentId, int authorId, string content)
    {
        var response = await http.PutAsJsonAsync($"api/tasks/{taskId}/comments/{commentId}", new UpdateCommentRequest(authorId, content));
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CommentDto>();
    }

    public async Task DeleteCommentAsync(int taskId, int commentId, int authorId)
    {
        var response = await http.DeleteAsync($"api/tasks/{taskId}/comments/{commentId}?authorId={authorId}");
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        var message = string.IsNullOrWhiteSpace(body)
            ? $"{(int)response.StatusCode} {response.ReasonPhrase}"
            : body;

        throw new InvalidOperationException(message);
    }
}
