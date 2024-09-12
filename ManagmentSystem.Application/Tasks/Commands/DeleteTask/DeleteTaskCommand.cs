using ManagmentSystem.Core.Shared;
using MediatR;

namespace ManagmentSystem.Application.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid TaskId, Guid OwnerId) : IRequest<Result<Guid>>;
