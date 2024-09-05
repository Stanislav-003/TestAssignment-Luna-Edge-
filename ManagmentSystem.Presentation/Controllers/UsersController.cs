using ManagmentSystem.Application.Services;
using ManagmentSystem.Application.Users.Commands.CreateUser;
using ManagmentSystem.Core.Abstractions;
using ManagmentSystem.Presentation.Contracts;
using MediatR;
using ManagmentSystem.Core.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManagmentSystem.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator, IUsersService usersService)
        {
            _usersService = usersService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<UsersResponse>>> GetUsers()
        {
            var users = await _usersService.GetUsers();

            var response = users.Select(u => new UsersResponse(u.Id, u.UserName, u.Email));

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UsersRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand(request.userName, request.email, request.password);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result.Error.Message);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateBook(Guid id, [FromBody] UsersRequest request)
        {
            var userId = await _usersService.UpdateUser(id, request.userName, request.email, request.password);

            return Ok(userId);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            return Ok(await _usersService.DeleteUser(id));
        }
    }
}
