using ManagmentSystem.Application.Abstractions;
using ManagmentSystem.Application.Abstractions.Auth;
using ManagmentSystem.Application.Abstractions.Messaging;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ManagmentSystem.Application.Users.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, string>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IUsersRepository usersRepository, 
        IPasswordHasher passwordHasher, 
        IJwtProvider jwtProvider)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Failure<string>(new Error("User.NotFound", "User not found."));
        }

        var result = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (result == false)
        {
            throw new Exception("Failed to login!");
        }

        var token = _jwtProvider.GenerateToken(user);

        return token;
    }
}
