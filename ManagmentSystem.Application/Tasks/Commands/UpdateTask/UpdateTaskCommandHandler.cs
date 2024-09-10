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
    private readonly ILogger<UpdateTaskCommandHandler> _logger;

    public UpdateTaskCommandHandler(ITasksRepository tasksRepository, ILogger<UpdateTaskCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _tasksRepository = tasksRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
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
            _logger.LogWarning("Failed to update task: {Error}", updateResult.Error);
            return Result.Failure<Guid>(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task with ID {TaskId} successfully updated", request.TaskId);

        return updateResult;
    }
}
