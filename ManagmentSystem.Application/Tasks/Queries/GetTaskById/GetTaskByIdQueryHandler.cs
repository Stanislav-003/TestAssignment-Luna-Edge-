using ManagmentSystem.Application.Abstractions.Messaging;
using ManagmentSystem.Application.Tasks.Queries.GetTasks;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagmentSystem.Application.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler : IQueryHandler<GetTaskByIdQuery, TaskByIdResponse>
{
    private readonly ITasksRepository _tasksRepository;

    public GetTaskByIdQueryHandler(ITasksRepository tasksRepository)
    {
        _tasksRepository = tasksRepository;
    }

    public async Task<Result<TaskByIdResponse>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var userTask = await _tasksRepository.GetById(request.userId, request.taskId);

        if (userTask == null)
        {
            return Result.Failure<TaskByIdResponse>(new Error("TaskNotFound", "Task not found or user is not authorized to view this task."));
        }

        var response = new TaskByIdResponse(
            userTask!.Id,
            userTask.Title,
            userTask.Description ?? string.Empty,
            userTask.DueDate ?? default,
            userTask.Status,
            userTask.Priority,
            userTask.CreatedAt,
            userTask.UpdatedAt);

        return Result.Success(response);
    }
}
