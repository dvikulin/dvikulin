using Taskify.Database.Models;
using TaskStatusModel = Taskify.Database.Models.TaskStatus;

namespace Taskify.Api.Validation;

public static class TaskifyValidation
{
    public static string? ValidateLength(string? value, string fieldName, int maxLength, bool required)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return required ? $"{fieldName} is required." : null;
        }

        return value.Length > maxLength
            ? $"{fieldName} must be {maxLength} characters or fewer."
            : null;
    }

    public static bool TryParseStatus(string? value, out TaskStatusModel status)
    {
        status = TaskStatusModel.Todo;
        var normalized = Normalize(value);

        if (normalized is "todo")
        {
            status = TaskStatusModel.Todo;
            return true;
        }

        if (normalized is "inprogress")
        {
            status = TaskStatusModel.InProgress;
            return true;
        }

        if (normalized is "inreview")
        {
            status = TaskStatusModel.InReview;
            return true;
        }

        if (normalized is "done")
        {
            status = TaskStatusModel.Done;
            return true;
        }

        return false;
    }

    public static bool TryParsePriority(string? value, out TaskPriority priority)
    {
        priority = TaskPriority.Medium;
        return string.IsNullOrWhiteSpace(value)
            || Enum.TryParse(value, ignoreCase: true, out priority);
    }

    private static string Normalize(string? value) =>
        new((value ?? string.Empty)
            .Where(char.IsLetterOrDigit)
            .Select(char.ToLowerInvariant)
            .ToArray());
}
