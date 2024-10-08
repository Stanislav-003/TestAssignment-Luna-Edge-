﻿using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using MediatR;

namespace ManagmentSystem.Application.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<Guid>>
{
    private readonly ITasksRepository _tasksRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(ITasksRepository tasksRepository, IUnitOfWork unitOfWork)
    {
        _tasksRepository = tasksRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        if (request.TaskId == Guid.Empty)
        {
            return Result.Failure<Guid>(new Error("InvalidTaskId", "Task ID must be provided."));
        }

        if (request.OwnerId == Guid.Empty)
        {
            return Result.Failure<Guid>(new Error("InvalidOwnerId", "Owner ID must be provided."));
        }

        var deleteResult = await _tasksRepository.DeleteById(request.TaskId, request.OwnerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (deleteResult.IsFailure)
        {
            return Result.Failure<Guid>(deleteResult.Error);
        }

        return Result.Success(deleteResult.Value);
    }
}
