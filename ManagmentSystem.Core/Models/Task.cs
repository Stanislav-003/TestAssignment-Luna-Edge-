using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = ManagmentSystem.Core.Enums.TaskStatus;

namespace ManagmentSystem.Core.Models;

public class Task
{
    public const int MAX_TITLE_LENGTH = 200;
    public const int MAX_DESCRIPTION_LENGTH = 1000;

    private Task(Guid id, string title, string? description, DateTime? dueDate, TaskStatus status, TaskPriority priority)
    {
        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;
        Priority = priority;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public string Title { get; } = string.Empty;
    public string? Description { get; } = null;
    public DateTime? DueDate { get; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public static (Task? Task, Error? Error) Create(Guid id, string title, string? description, DateTime? dueDate, string statusString, string priorityString)
    {
        if (string.IsNullOrEmpty(title) || title.Length > MAX_TITLE_LENGTH)
        {
            return (null, new Error("InvalidTitle", $"Title cannot be empty or longer than {MAX_TITLE_LENGTH} characters."));
        }

        if (description != null && description.Length > MAX_DESCRIPTION_LENGTH)
        {
            return (null, new Error("InvalidDescription", $"Description cannot be longer than {MAX_DESCRIPTION_LENGTH} characters."));
        }

        if (!Enum.TryParse<TaskStatus>(statusString, true, out var status) || !Enum.IsDefined(typeof(TaskStatus), status))
        {
            return (null, new Error("InvalidStatus", $"Status '{statusString}' is not a valid TaskStatus."));
        }

        if (!Enum.TryParse<TaskPriority>(priorityString, true, out var priority) || !Enum.IsDefined(typeof(TaskPriority), priority))
        {
            return (null, new Error("InvalidPriority", $"Priority '{priorityString}' is not a valid TaskPriority."));
        }

        var task = new Task(id, title, description, dueDate, status, priority);

        return (task, Error.None);
    }
}
