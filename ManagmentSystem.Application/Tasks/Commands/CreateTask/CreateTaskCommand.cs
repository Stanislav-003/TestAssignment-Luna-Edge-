using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Shared;
using MediatR;

namespace ManagmentSystem.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Title, 
    string Description, 
    DateTime DueDate, 
    string Status, 
    string Priority, 
    Guid UserId) : IRequest<Result<Guid>>;