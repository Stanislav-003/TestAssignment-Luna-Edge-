using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Core.Models;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ManagmentSystem.Application.Users.Commands.DeleteUser
{
    public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Guid>>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUsersRepository usersRepository, IUnitOfWork unitOfWork)
        {
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetById(request.userId);

            if (user == null)
            {
                return Result.Failure<Guid>(new Error("User.NotFound", "User not found."));
            }

            var deletedUserId = await _usersRepository.Delete(request.userId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(deletedUserId);
        }
    }
}
