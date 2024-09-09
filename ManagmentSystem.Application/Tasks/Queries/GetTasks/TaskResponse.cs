using ManagmentSystem.Core.Enums;

namespace ManagmentSystem.Application.Tasks.Queries.GetTasks;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime DueDate,
    Core.Enums.TaskStatus Status,
    TaskPriority Priority,
    DateTime CreatedAt,
    DateTime UpdatedAt);