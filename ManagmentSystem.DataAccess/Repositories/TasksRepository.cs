using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagmentSystem.DataAccess.Repositories;

public class TasksRepository : ITasksRepository
{
    private readonly ManagmentSystemDbContext _context;

    public TasksRepository(ManagmentSystemDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Create(Core.Models.Task task, Guid creatorId, CancellationToken cancellationToken = default)
    {
        var taskEntity = new TaskEntity 
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority,
            UserId = creatorId
        };

        await _context.Tasks.AddAsync(taskEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return taskEntity.Id;
    }

    public async Task<List<TaskEntity>> GetAll(Guid OwnerId, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Where(t => t.UserId == OwnerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskEntity?> GetById(Guid OwnerId, Guid TaskId, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == OwnerId && t.Id == TaskId, cancellationToken);
    }
}
