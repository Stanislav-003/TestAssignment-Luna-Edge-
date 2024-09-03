namespace ManagementSystem.Contracts
{
    public record UsersResponse(
        Guid id,
        string userName,
        string email);
}
