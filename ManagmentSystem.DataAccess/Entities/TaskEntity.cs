using ManagmentSystem.Core.Enums;
using TaskStatus = ManagmentSystem.Core.Enums.TaskStatus;

namespace ManagmentSystem.DataAccess.Entities
{
    public class TaskEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; } = DateTime.UtcNow;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
    }
}
