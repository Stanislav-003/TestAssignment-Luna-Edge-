using ManagmentSystem.Application.Users.Login;
using ManagmentSystem.Application.Users.Register;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagmentSystem.Presentation.Controllers;

public class LoginController : ApiController
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator) : base(mediator)
    {
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
}
