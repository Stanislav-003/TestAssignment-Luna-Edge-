using ManagmentSystem.Application.Abstractions.Messaging;
using ManagmentSystem.Application.Tasks.Queries.GetTasks;

namespace ManagmentSystem.Application.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid userId, Guid taskId) : IQuery<TaskByIdResponse>;
