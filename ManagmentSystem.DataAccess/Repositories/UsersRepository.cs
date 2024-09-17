using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagmentSystem.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ManagmentSystemDbContext _context;

    public UsersRepository(ManagmentSystemDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>?> GetAll(CancellationToken cancellationToken = default)
    {
        var usersEntities = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);

        if (usersEntities == null)
        {
            return null;
        }

        var users = usersEntities.Select(u => User.Create(u.Id, u.UserName, u.Email, u.PasswordHash).User).ToList();

        return users!;
    }

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    { 
        var userEntity = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (userEntity == null)
        {
            return null; 
        }

        var user = User.Create(userEntity.Id, userEntity.UserName, userEntity.Email, userEntity.PasswordHash).User;

        return user;
    }

    public async Task<UserEntity?> GetByEmailAsync(string Email, CancellationToken cancellationToken = default)
    {
        var userEntity = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == Email, cancellationToken);

        return userEntity;
    }

    public async Task<Guid> Create(User user, string passwordHash, CancellationToken cancellationToken)
    {
        var userEntity = new UserEntity 
        {
            UserName = user.UserName,
            Email = user.Email,
            PasswordHash = passwordHash,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };

        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync(cancellationToken);

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
