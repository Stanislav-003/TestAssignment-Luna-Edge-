using MediatR;
using ManagmentSystem.Core.Shared;
using Microsoft.AspNetCore.Mvc;
using ManagmentSystem.Application.Users.Login;
using ManagmentSystem.Presentation.Abstractions;
using Microsoft.AspNetCore.Authorization;
using ManagmentSystem.Application.Users.Register;
using Microsoft.AspNetCore.Http;
using ManagmentSystem.Application.Tasks.Commands.CreateTask;
using ManagmentSystem.Application.Tasks.Queries.GetTasks;
using ManagmentSystem.Application.Tasks.Queries.GetTaskById;
using ManagmentSystem.Application.Tasks.Commands.UpdateTask;
using ManagmentSystem.Application.Abstractions.Auth;
using ManagmentSystem.Application.Tasks.Commands.DeleteTask;
using ManagmentSystem.Presentation.ActionFilters;

namespace ManagmentSystem.Presentation.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserContextService _userContextService;
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator, IUserContextService userContextService) : base(mediator)
        {
            _userContextService = userContextService;
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("users/register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(
                request.userName,
                request.email,
                request.password);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
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

        //[Authorize]
        [HttpPost("users/logout")]
        public IActionResult LogoutUser()
        {
            if (HttpContext.Request.Cookies.ContainsKey("test-cookies"))
            {
                HttpContext.Response.Cookies.Delete("test-cookies");
            }

            return Ok("User has been logged out successfully.");
        }

        //[Authorize]
        [ServiceFilter(typeof(EnsureUserIdClaimFilterAttribute))]
        [HttpPost("tasks")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateTaskCommand(
                request.Title,
                request.Description,
                request.DueDate,
                request.Status,
                request.Priority,
                _userContextService.GetUserId()!.Value);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value);
        }

        //[Authorize]
        [ServiceFilter(typeof(EnsureUserIdClaimFilterAttribute))]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
        {
            var query = new GetTasksQuery(_userContextService.GetUserId()!.Value);

            Result<TasksResponse> response = await _mediator.Send(query, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Value)
                : NotFound(response.Error);
        }

        //[Authorize]
        [ServiceFilter(typeof(EnsureUserIdClaimFilterAttribute))]
        [HttpGet("task/{id}")]
        public async Task<IActionResult> GetTaskById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Task ID is required.");
            }

            var query = new GetTaskByIdQuery(_userContextService.GetUserId()!.Value, id);

            Result<TaskByIdResponse> response = await _mediator.Send(query, cancellationToken);

            return response.IsSuccess
                ? Ok(response.Value)
                : NotFound(response.Error);
        }

        //[Authorize]
        [ServiceFilter(typeof(EnsureUserIdClaimFilterAttribute))]
        [HttpPut("task/id")]
        public async Task<IActionResult> UpdateTaskById([FromRoute] Guid id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Task ID is required.");
            }

            var command = new UpdateTaskCommand(
                id, 
                request.Title,
                request.Description,
                request.DueDate,
                request.Status,
                request.Priority,
                _userContextService.GetUserId()!.Value);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value);
        }

        [HttpDelete("task/delete")]
        [ServiceFilter(typeof(EnsureUserIdClaimFilterAttribute))]
        public async Task<IActionResult> DeleteTaskByID([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Task ID is required.");
            }

            var command = new DeleteTaskCommand(id, _userContextService.GetUserId()!.Value);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result.Value);
        }
    }
}
