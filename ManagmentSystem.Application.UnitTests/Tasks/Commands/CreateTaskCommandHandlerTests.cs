using Moq;
using ManagmentSystem.Core.Models;
using ManagmentSystem.DataAccess.Abstractions;
using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Application.Tasks.Commands.CreateTask;
using Task = ManagmentSystem.Core.Models.Task;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Specialized;
using ManagmentSystem.Core.Shared;

namespace ManagmentSystem.Application.UnitTests.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITasksRepository> _tasksRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _tasksRepositoryMock = new Mock<ITasksRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateTaskCommandHandler(_tasksRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidRequest_ShouldReturnSuccessWithTaskId()
    {
        // Arrange
        var command = new CreateTaskCommand("Valid Title", "Valid Description", DateTime.UtcNow, "Pending", "Low", Guid.NewGuid());
        var taskId = Guid.NewGuid();

        // Мокаємо метод створення завдання
        _tasksRepositoryMock
            .Setup(repo => repo.Create(It.IsAny<Core.Models.Task>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskId);

        // Мокаємо метод збереження змін у UnitOfWork
        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(taskId);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_InvalidTitle_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateTaskCommand("", "Valid Description", DateTime.UtcNow, "Pending", "High", Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Code.Should().Be("InvalidTitle");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_DescriptionTooLong_ShouldReturnFailure()
    {
        // Arrange
        var longDescription = new string('a', 1001); 
        var command = new CreateTaskCommand("Valid Title", longDescription, DateTime.UtcNow, "Pending", "High", Guid.NewGuid());

        // Мокаємо метод збереження змін у UnitOfWork
        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse(); 
        result.Error.ToString().Should().Contain("InvalidDescription"); 
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_InvalidStatus_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateTaskCommand("Valid Title", "Valid Description", DateTime.UtcNow, "InvalidStatus", "High", Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Code.Should().Be("InvalidStatus");
    }

    /// <summary>
    /// тест імітує позаштатну ситуацію (помилку в репозиторії) і перевіряє, що хендлер 
    /// правильно працює з такими ситуаціями, що важливо для забезпечення стійкості 
    /// системи до помилок у нижчих шарах (наприклад, у базі даних)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async System.Threading.Tasks.Task Handle_RepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateTaskCommand("Valid Title", "Valid Description", DateTime.UtcNow, "Pending", "High", Guid.NewGuid());
        var expectedExceptionMessage = "Database error";

        // Налаштування мока для викиду помилки
        _tasksRepositoryMock
            .Setup(repo => repo.Create(It.IsAny<Core.Models.Task>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        // Act
        Func<Task<Result<Guid>>> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage(expectedExceptionMessage);
    }
}
