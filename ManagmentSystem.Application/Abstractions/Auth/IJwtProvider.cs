using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Entities;

namespace ManagmentSystem.Application.Abstractions.Auth;

public interface IJwtProvider
{
    string GenerateToken(UserEntity user);
}
