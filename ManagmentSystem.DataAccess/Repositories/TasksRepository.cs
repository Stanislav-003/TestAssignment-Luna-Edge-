using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Models;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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

    public async Task<Result<Guid>> UpdateById(Guid TaskId, string Title, string Description, DateTime DueDate, Core.Enums.TaskStatus Status, TaskPriority Priority, Guid UserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == TaskId && t.UserId == UserId, cancellationToken);

        if (task == null)
        {
            var error = new Error("TaskNotFound", "Task not found or user is not authorized to update this task.");
            return Result.Failure<Guid>(error);
        }

        task.Title = Title;
        task.Description = Description;
        task.DueDate = DueDate;
        task.Status = Status;
        task.Priority = Priority;
        task.UpdatedAt = DateTime.UtcNow; // Update DateTime

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(task.Id);
    }

    public async Task<Result<Guid>> DeleteById(Guid TaskId, Guid UserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == TaskId && t.UserId == UserId, cancellationToken);

        if (task == null)
        {
            var error = new Error("TaskNotFound", "Task not found or user is not authorized to update this task.");
            return Result.Failure<Guid>(error);
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(task.Id);
    }
}
