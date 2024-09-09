using ManagmentSystem.Core.Enums;
using ManagmentSystem.DataAccess.Entities;

namespace ManagmentSystem.Application.Tasks.Queries.GetTasks;

public sealed record TasksResponse(
    IEnumerable<TaskResponse> Tasks);
