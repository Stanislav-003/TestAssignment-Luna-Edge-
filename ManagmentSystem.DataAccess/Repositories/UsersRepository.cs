using ManagmentSystem.Core.Abstractions;
using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagmentSystem.DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ManagmentSystemDbContext _context;

        public UsersRepository(ManagmentSystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Get()
        {
            var userEntities = await _context.Users.AsNoTracking().ToListAsync();

            var users = userEntities.Select(u => User.Create(u.Id, u.UserName, u.Email, u.PasswordHash).User).ToList();

            return users;
        }

        public async Task<Guid> Create(User user)
        {
            var userEntity = new UserEntity
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
            };

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            return userEntity.Id;
        }

        public async Task<Guid> Update(Guid id, string userName, string email, string passwordHash)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.UserName, b => userName)
                    .SetProperty(b => b.Email, b => email)
                    .SetProperty(b => b.PasswordHash, b => passwordHash));

            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}
