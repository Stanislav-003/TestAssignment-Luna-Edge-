using ManagmentSystem.Application.Abstractions.Auth;
using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Application.Abstractions.Messaging;
using ManagmentSystem.Core.Models;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.DataAccess.Entities;
using ManagmentSystem.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ManagmentSystem.Application.Users.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IPasswordHasher passwordHasher
        ,IUsersRepository usersRepository
        ,IUnitOfWork unitOfWork)
    {
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (user, error) = User.Create(Guid.NewGuid(), request.userName, request.email, request.password);

        if (user == null || error != Error.None)
        {
            return Result.Failure<Guid>(error ?? new Error("UnknownError", "Failed to create user."));
        }

        var passwordHash = _passwordHasher.Generate(user!.Password);

        var userId = await _usersRepository.Create(user!, passwordHash, cancellationToken);

        //await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(userId);
    }
}
