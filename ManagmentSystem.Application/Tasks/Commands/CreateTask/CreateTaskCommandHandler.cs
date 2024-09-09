using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using MediatR;

namespace ManagmentSystem.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITasksRepository _tasksRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(ITasksRepository tasksRepository, IUnitOfWork unitOfWork)
    {
        _tasksRepository = tasksRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var (task, error) = Core.Models.Task.Create(
            Guid.NewGuid(), 
            request.Title, 
            request.Description, 
            request.DueDate, 
            request.Status,
            request.Priority);

        if (task == null || error != Error.None)
        {
            return Result.Failure<Guid>(error ?? new Error("UnknownError", "Failed to create user."));
        }

        var taskId = await _tasksRepository.Create(task, request.UserId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(taskId);
    }
}
