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

            builder.Property(b => b.UserName).HasMaxLength(User.MAX_USERNAME_LENGTH).IsRequired();

            builder.Property(b => b.Email).HasMaxLength(User.MAX_EMAIL_LENGTH).IsRequired();

            builder.Property(b => b.PasswordHash).HasMaxLength(User.MAX_PASSWORD_LENGTH).IsRequired();
        }
    }
}
