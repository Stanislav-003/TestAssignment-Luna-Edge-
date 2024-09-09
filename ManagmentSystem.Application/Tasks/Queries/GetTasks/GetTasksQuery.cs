using ManagmentSystem.Application.Abstractions.Messaging;

namespace ManagmentSystem.Application.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery(Guid userId) : IQuery<TasksResponse>;