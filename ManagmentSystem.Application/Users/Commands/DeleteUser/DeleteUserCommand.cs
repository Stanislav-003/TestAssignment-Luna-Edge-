using ManagmentSystem.Core.Models;
using ManagmentSystem.Core.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagmentSystem.Application.Users.Commands.DeleteUser
{
    public sealed record DeleteUserCommand(Guid userId) : IRequest<Result<Guid>>; 
}
