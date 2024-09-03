using ManagmentSystem.Core.Abstractions;
using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Repositories;

namespace ManagmentSystem.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _usersRepository.Get();
        }

        public async Task<Guid> CreateUser(User user)
        {
            return await _usersRepository.Create(user);
        }

        public async Task<Guid> UpdateUser(Guid id, string userName, string email, string passwordHash)
        {
            return await _usersRepository.Update(id, userName, email, passwordHash);
        }

        public async Task<Guid> DeleteUser(Guid id)
        {
            return await _usersRepository.Delete(id);
        }

    }
}
