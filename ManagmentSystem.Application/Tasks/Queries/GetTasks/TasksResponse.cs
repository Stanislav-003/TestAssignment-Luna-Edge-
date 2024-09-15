using ManagmentSystem.Core.Enums;
using ManagmentSystem.DataAccess.Entities;
using ManagmentSystem.DataAccess;

namespace ManagmentSystem.Application.Tasks.Queries.GetTasks;

public sealed record TasksResponse(
    PagedList<TaskResponse> Tasks);
