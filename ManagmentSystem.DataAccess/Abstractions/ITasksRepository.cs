﻿using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Entities;

namespace ManagmentSystem.DataAccess.Abstractions;

public interface ITasksRepository
{
    Task<Guid> Create(Core.Models.Task task, Guid creatorId, CancellationToken cancellationToken = default);
    Task<PagedList<TaskEntity>> GetAll(Guid OwnerId, int page, int pageSize, string? SearchTerm = null, string? sortColumn = null, string? sortOrder = null, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetById(Guid OwnerId, Guid TaskId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> UpdateById(Guid TaskId, string Title, string Description, DateTime DueDate, Core.Enums.TaskStatus Status, TaskPriority Priority, Guid UserId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> DeleteById(Guid TaskId, Guid UserId, CancellationToken cancellationToken = default);
}