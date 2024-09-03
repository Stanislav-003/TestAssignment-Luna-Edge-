namespace ManagementSystem.Contracts
{
    public record UsersRequest(
        string userName,
        string email,
        string password);
}
