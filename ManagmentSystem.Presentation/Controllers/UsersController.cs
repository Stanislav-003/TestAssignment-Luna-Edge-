using ManagmentSystem.Application.Users.Commands.CreateUser;
using ManagmentSystem.Presentation.Contracts;
using MediatR;
using ManagmentSystem.Core.Shared;
using Microsoft.AspNetCore.Mvc;
using ManagmentSystem.Application.Users.Commands.DeleteUser;
using ManagmentSystem.Application.Users.Login;
using ManagmentSystem.Presentation.Abstractions;
using Microsoft.AspNetCore.Authorization;
using ManagmentSystem.Application.Users.Register;
using Microsoft.AspNetCore.Http;
using ManagmentSystem.Application.Tasks.Commands.CreateTask;
using ManagmentSystem.Application.Tasks.Queries.GetTasks;
using ManagmentSystem.Application.Tasks.Queries.GetTaskById;

namespace ManagmentSystem.Presentation.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("users/register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request.userName, request.email, request.password);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("users/login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.Email, request.Password);

            Result<string> tokenResult = await _mediator.Send(command, cancellationToken);

            if (tokenResult.IsFailure)
            { 
                return HandleFailure(tokenResult);
            }

            HttpContext.Response.Cookies.Append("test-cookies", tokenResult.Value);

            return Ok();
        }

        [Authorize]
        [HttpPost("tasks")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = HttpContext.User.FindFirst("userId");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var creatorId))
            {
                return Unauthorized("User ID is not available in the token.");
            }

            var command = new CreateTaskCommand(request.Title, request.Description, request.DueDate, request.Status, request.Priority, creatorId);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
        {
            var userIdClaim = HttpContext.User.FindFirst("userId");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var creatorId))
            {
                return Unauthorized("User ID is not available in the token.");
            }

            var query = new GetTasksQuery(creatorId);

            Result<TasksResponse> response = await _mediator.Send(query, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Value)
                : NotFound(response.Error);
        }

        [Authorize]
        [HttpGet("task/id")]
        public async Task<IActionResult> GetTaskById([FromQuery]Guid taskId, CancellationToken cancellationToken)
        {
            if (taskId == Guid.Empty)
            {
                return BadRequest("Task ID is required.");
            }

            var userIdClaim = HttpContext.User.FindFirst("userId");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var creatorId))
            {
                return Unauthorized("User ID is not available in the token.");
            }

            var query = new GetTaskByIdQuery(creatorId, taskId);

            Result<TaskByIdResponse> response = await _mediator.Send(query, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Value)
                : NotFound(response.Error);
        }



        //[Authorize]
        //[HttpPost]
        //public async Task<ActionResult<Guid>> CreateUser([FromBody] UsersRequest request, CancellationToken cancellationToken)
        //{
        //    var command = new CreateUserCommand(request.userName, request.email, request.password);

        //    var result = await _mediator.Send(command, cancellationToken);

        //    if (result.IsFailure)
        //    {
        //        return BadRequest(result.Error.Message);
        //    }

        //    return Ok(result.Value);
        //}

        //[Authorize]
        //[HttpDelete("{id:guid}")]
        //public async Task<ActionResult<Guid>> DeleteUser(Guid id, CancellationToken cancellationToken)
        //{
        //    var command = new DeleteUserCommand(id);

        //    var result = await _mediator.Send(command, cancellationToken);

        //    if (result.IsFailure)
        //    {
        //        if (result.Error == Error.NullValue)
        //        {
        //            return NotFound($"User with ID {id} not found.");
        //        }

        //        return BadRequest(result.Error.Message);
        //    }

        //    return Ok(result.Value);
        //}
    }
}
