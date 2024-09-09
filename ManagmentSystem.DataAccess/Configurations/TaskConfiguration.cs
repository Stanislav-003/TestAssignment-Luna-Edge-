using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = ManagmentSystem.Core.Models.Task;

namespace ManagmentSystem.DataAccess.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(Task.MAX_TITLE_LENGTH);

            builder.Property(t => t.Description)
                .HasMaxLength(Task.MAX_DESCRIPTION_LENGTH);

            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.Priority)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .ValueGeneratedNever();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade); // При видаленні користувача видаляються його завдання
        }
    }
}
