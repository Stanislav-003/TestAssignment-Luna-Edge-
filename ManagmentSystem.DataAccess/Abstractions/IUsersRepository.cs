using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Entities;

namespace ManagmentSystem.DataAccess.Abstractions;

public interface IUsersRepository
{
    Task<List<User>?> GetAll(CancellationToken cancellationToken = default);
    Task<User?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string Email, CancellationToken cancellationToken = default);
    Task<Guid> Create(User user, string passwordHash, CancellationToken cancellationToken = default);
    Task<Guid> Delete(Guid id);
    Task<Guid> Update(Guid id, string userName, string email, string passwordHash);
}
