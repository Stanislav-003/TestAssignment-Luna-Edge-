using ManagmentSystem.Core.Models;

namespace ManagmentSystem.Core.Abstractions
{
    public interface IUsersRepository
    {
        Task<Guid> Create(User user);
        Task<Guid> Delete(Guid id);
        Task<List<User>> Get();
        Task<Guid> Update(Guid id, string userName, string email, string passwordHash);
    }
}