using ManagmentSystem.Application.Abstractions.Messaging;

namespace ManagmentSystem.Application.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery(
    Guid userId, 
    int page,
    int pageSize,
    string? SearchTerm = null,
    string? sortColumn = null,
    string? sortOrder = null) : IQuery<TasksResponse>;