using Taskify.Api.Dtos;
using Taskify.Api.Validation;
using Taskify.Database.Models;
using TaskStatusModel = Taskify.Database.Models.TaskStatus;

namespace Taskify.Tests;

public sealed class TaskifyValidationTests
{
    [Theory]
    [InlineData("To Do", TaskStatusModel.Todo)]
    [InlineData("In Progress", TaskStatusModel.InProgress)]
    [InlineData("In Review", TaskStatusModel.InReview)]
    [InlineData("Done", TaskStatusModel.Done)]
    public void TryParseStatusAcceptsKanbanLabels(string value, TaskStatusModel expected)
    {
        var parsed = TaskifyValidation.TryParseStatus(value, out var status);

        Assert.True(parsed);
        Assert.Equal(expected, status);
    }

    [Fact]
    public void TryParseStatusRejectsInvalidStatus()
    {
        var parsed = TaskifyValidation.TryParseStatus("Blocked", out _);

        Assert.False(parsed);
    }

    [Fact]
    public void ValidateLengthRequiresNonWhitespaceValues()
    {
        var error = TaskifyValidation.ValidateLength(" ", "Task title", 300, required: true);

        Assert.Equal("Task title is required.", error);
    }

    [Fact]
    public void TaskMappingUsesHumanReadableKanbanStatus()
    {
        var task = new Taskify.Database.Models.Task
        {
            Id = 1,
            ProjectId = 1,
            Title = "Review board",
            Status = TaskStatusModel.InReview,
            Priority = TaskPriority.High
        };

        var dto = task.ToDto();

        Assert.Equal("In Review", dto.Status);
        Assert.Equal("High", dto.Priority);
    }
}
