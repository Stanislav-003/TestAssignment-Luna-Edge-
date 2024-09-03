using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Configurations;
using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagmentSystem.DataAccess
{
    public class ManagmentSystemDbContext : DbContext
    {
        public ManagmentSystemDbContext(DbContextOptions<ManagmentSystemDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
