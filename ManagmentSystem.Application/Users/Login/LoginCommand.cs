using ManagmentSystem.Application.Abstractions.Messaging;

namespace ManagmentSystem.Application.Users.Login;

public record LoginCommand(string Email, string Password) : ICommand<string>;
