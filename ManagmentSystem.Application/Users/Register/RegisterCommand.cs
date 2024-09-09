using ManagmentSystem.Application.Abstractions.Messaging;
using System.Windows.Input;

namespace ManagmentSystem.Application.Users.Register;

public record RegisterCommand(string userName, string email, string password) : Abstractions.Messaging.ICommand;
