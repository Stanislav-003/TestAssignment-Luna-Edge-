using ManagmentSystem.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ManagmentSystem.Application.Auth;

namespace ManagmentSystem.Presentation.ActionFilters;

public class EnsureUserIdClaimFilterAttribute : Attribute, IAsyncActionFilter
{
    private readonly IUserContextService _userContextService;

    public EnsureUserIdClaimFilterAttribute(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userIdClaim = _userContextService.GetUserId();

        if (userIdClaim == null)
        {
            context.Result = new UnauthorizedObjectResult("User ID is not available in the token.");
            return;
        }

        await next();
    }
}