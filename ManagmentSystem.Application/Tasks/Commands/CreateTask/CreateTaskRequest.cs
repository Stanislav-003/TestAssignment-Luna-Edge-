using ManagmentSystem.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ManagmentSystem.Application.Tasks.Commands.CreateTask;

public record CreateTaskRequest(
    string Title,
    string Description,
    DateTime DueDate,
    Core.Enums.TaskStatus Status,
    TaskPriority Priority);
