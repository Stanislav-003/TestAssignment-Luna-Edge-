using ManagmentSystem.Application.Abstractions.Messaging;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using System.Linq;

namespace ManagmentSystem.Application.Tasks.Queries.GetTasks;

public sealed class GetTasksQueryHandler : IQueryHandler<GetTasksQuery, TasksResponse>
{
    private readonly ITasksRepository _tasksRepository;

    public GetTasksQueryHandler(ITasksRepository tasksRepository)
    {
        _tasksRepository = tasksRepository;
    }

    public async Task<Result<TasksResponse>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var userTasks = await _tasksRepository.GetAll(request.userId);

        var response = new TasksResponse(
            userTasks.Select(task => new TaskResponse(
                task.Id,
                task.Title,
                task.Description ?? string.Empty,
                task.DueDate ?? default,
                task.Status,
                task.Priority,
                task.CreatedAt,
                task.UpdatedAt
            ))
        );

        return Result.Success(response);
    }
}
