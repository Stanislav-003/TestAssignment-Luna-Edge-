using ManagmentSystem.Core.Enums;

namespace ManagmentSystem.Application.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskRequest(
    string Title,
    string Description,
    DateTime DueDate,
    Core.Enums.TaskStatus Status,
    TaskPriority Priority);
