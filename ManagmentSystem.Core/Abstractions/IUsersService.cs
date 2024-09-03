using ManagmentSystem.Core.Models;

namespace ManagmentSystem.Core.Abstractions
{
    public interface IUsersService
    {
        Task<Guid> CreateUser(User user);
        Task<Guid> DeleteUser(Guid id);
        Task<List<User>> GetUsers();
        Task<Guid> UpdateUser(Guid id, string userName, string email, string passwordHash);
    }
}