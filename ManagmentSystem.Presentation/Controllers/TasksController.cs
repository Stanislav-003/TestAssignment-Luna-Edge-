using ManagmentSystem.Application.Abstractions.Auth;
using ManagmentSystem.Application.Tasks.Commands.CreateTask;
using ManagmentSystem.Application.Tasks.Commands.DeleteTask;
using ManagmentSystem.Application.Tasks.Commands.UpdateTask;
using ManagmentSystem.Application.Tasks.Queries.GetTaskById;
using ManagmentSystem.Application.Tasks.Queries.GetTasks;
using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.Presentation.Abstractions;
using ManagmentSystem.Presentation.ActionFilters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = ManagmentSystem.Core.Enums.TaskStatus;

namespace ManagmentSystem.Presentation.Controllers;

public class TasksController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IUserContextService _userContextService;

    public TasksController(IMediator mediator, IUserContextService userContextService) : base(mediator)
    {
        _mediator = mediator;
        _userContextService = userContextService;
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
    public async Task<IActionResult> GetTasks(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortColumn = null,
        [FromQuery] string? sortOrder = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTasksQuery(
            _userContextService.GetUserId()!.Value,
            page,
            pageSize,
            searchTerm,
            sortColumn,
            sortOrder);

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
