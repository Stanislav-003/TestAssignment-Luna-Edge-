namespace ManagmentSystem.Application.Abstractions.Auth;

public interface IUserContextService
{
    Guid? GetUserId();
}
