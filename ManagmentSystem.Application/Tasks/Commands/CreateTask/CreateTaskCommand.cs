using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Shared;
using MediatR;

namespace ManagmentSystem.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Title, 
    string Description, 
    DateTime DueDate, 
    Core.Enums.TaskStatus Status, 
    TaskPriority Priority, 
    Guid UserId) : IRequest<Result<Guid>>;