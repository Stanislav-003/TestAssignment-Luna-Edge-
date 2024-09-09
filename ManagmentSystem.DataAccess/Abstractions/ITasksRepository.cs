using ManagmentSystem.DataAccess.Entities;

namespace ManagmentSystem.DataAccess.Abstractions;

public interface ITasksRepository
{
    Task<Guid> Create(Core.Models.Task task, Guid creatorId, CancellationToken cancellationToken = default);
    Task<List<TaskEntity>> GetAll(Guid OwnerId, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetById(Guid OwnerId, Guid TaskId, CancellationToken cancellationToken = default);
}
