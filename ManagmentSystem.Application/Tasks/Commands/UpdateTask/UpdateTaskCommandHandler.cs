using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.DataAccess.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ManagmentSystem.Application.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<Guid>>
{
    private readonly ITasksRepository _tasksRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskCommandHandler(ITasksRepository tasksRepository, IUnitOfWork unitOfWork)
    {
        _tasksRepository = tasksRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result.Failure<Guid>(new Error("InvalidUserId", "User ID must be provided."));
        }

        var updateResult = await _tasksRepository.UpdateById(
            request.TaskId,
            request.Title,
            request.Description,
            request.DueDate,
            request.Status,
            request.Priority,
            request.UserId, 
            cancellationToken);

        if (updateResult.IsFailure)
        {
            return Result.Failure<Guid>(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return updateResult;
    }
}
