using ManagmentSystem.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Http;

namespace ManagmentSystem.Application.Auth;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId");
        return Guid.TryParse(userIdClaim?.Value, out var userId) ? userId : null;
    }
}
