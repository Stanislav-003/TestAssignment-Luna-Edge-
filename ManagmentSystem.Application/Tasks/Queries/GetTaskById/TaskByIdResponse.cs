using ManagmentSystem.Core.Enums;

namespace ManagmentSystem.Application.Tasks.Queries.GetTaskById;

public sealed record TaskByIdResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime DueDate,
    Core.Enums.TaskStatus Status,
    TaskPriority Priority,
    DateTime CreatedAt,
    DateTime UpdatedAt);
