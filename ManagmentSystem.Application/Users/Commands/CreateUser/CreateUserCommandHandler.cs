using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Application.Abstractions.Messaging;
using ManagmentSystem.Core.Abstractions;
using ManagmentSystem.Core.Models;
using ManagmentSystem.Core.Shared;

namespace ManagmentSystem.Application.Users.Commands.CreateUser
{
    public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
        {
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var (user, error) = User.Create(
                Guid.NewGuid(),
                command.UserName,
                command.Email,
                command.Password
            );

            if (user != null && error != Error.None)
            {
                return Result.Failure<Guid>(error!); 
            }

            var userId = await _usersRepository.Create(user!, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(userId);
        }
    }
}
