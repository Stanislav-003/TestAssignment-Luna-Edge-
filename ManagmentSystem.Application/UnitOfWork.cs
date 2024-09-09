using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.DataAccess;

namespace ManagmentSystem.Application
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ManagmentSystemDbContext _dbContext;

        public UnitOfWork(ManagmentSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
