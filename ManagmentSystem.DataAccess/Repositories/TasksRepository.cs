using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Models;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Linq;
using System.Linq.Expressions;
using TaskStatus = ManagmentSystem.Core.Enums.TaskStatus;

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

    public async Task<PagedList<TaskEntity>> GetAll(
        Guid OwnerId,
        int page,
        int pageSize,
        string? searchTerm = null,
        string? sortColumn = null,
        string? sortOrder = null,
        CancellationToken cancellationToken = default)
    {
        // перевірка та встановлення значень за замовчуванням для сторінки та розміру сторінки
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        // формування базового запиту: вибираємо задачі, що належать конкретному користувачеві
        IQueryable<TaskEntity> tasksQuery = _context.Tasks.Where(t => t.UserId == OwnerId);

        // застосування фільтрації за пошуковим запитом
        tasksQuery = ApplySearch(tasksQuery, searchTerm);

        // застосування сортування за вказаною колонкою та порядком
        tasksQuery = ApplySorting(tasksQuery, sortColumn, sortOrder);

        // вимикаємо відстеження змін для покращення продуктивності
        var tasksResponesQuery = tasksQuery.AsNoTracking();

        // створення об'єкту PagedList з отриманих даних
        var tasks = await PagedList<TaskEntity>.CreateAsync(
            tasksResponesQuery,
            page,
            pageSize);

        return tasks;
    }

    // метод для застосування фільтрації за пошуковим запитом
    private IQueryable<TaskEntity> ApplySearch(IQueryable<TaskEntity> query, string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            TaskStatus? status = Enum.TryParse(searchTerm, true, out TaskStatus parsedStatus) ? parsedStatus : (TaskStatus?)null;
            TaskPriority? priority = Enum.TryParse(searchTerm, true, out TaskPriority parsedPriority) ? parsedPriority : (TaskPriority?)null;

            query = query.Where(t =>
                (status.HasValue && t.Status == status.Value) ||
                (priority.HasValue && t.Priority == priority.Value)
            );
        }

        return query;
    }

    // метод для застосування сортування за вказаною колонкою та порядком
    private IQueryable<TaskEntity> ApplySorting(IQueryable<TaskEntity> query, string? sortColumn, string? sortOrder)
    {
        Expression<Func<TaskEntity, object>> keySelector = sortColumn?.ToLower() switch
        {
            "status" => task => task.Status,
            "priority" => task => task.Priority,
            _ => task => task.Id,
        };

        return sortOrder?.ToLower() == "desc"
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
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
