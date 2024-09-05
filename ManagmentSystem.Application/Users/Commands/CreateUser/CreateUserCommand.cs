using ManagmentSystem.Application.Abstractions.Messaging;
using ManagmentSystem.Core.Models;
using ManagmentSystem.Core.Shared;

namespace ManagmentSystem.Application.Users.Commands.CreateUser
{
    public sealed record CreateUserCommand(string UserName, string Email, string Password) : ICommand<Result<Guid>>;
}
