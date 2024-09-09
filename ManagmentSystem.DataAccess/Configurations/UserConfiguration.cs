using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagmentSystem.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(b => b.UserName)
                .HasMaxLength(User.MAX_USERNAME_LENGTH)
                .IsRequired();

            builder.Property(b => b.Email)
                .HasMaxLength(User.MAX_EMAIL_LENGTH)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(b => b.PasswordHash)
                .HasMaxLength(User.MAX_PASSWORD_LENGTH)
                .IsRequired();

            builder.HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // При видаленні користувача видаляються і його завдання
        }
    }
}
