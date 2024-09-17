using FluentAssertions;
using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Application.Tasks.Commands.CreateTask;
using ManagmentSystem.Application.Tasks.Commands.DeleteTask;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using Moq;
using System.Reflection.Metadata;

namespace ManagmentSystem.Application.UnitTests.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITasksRepository> _tasksRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _tasksRepositoryMock = new Mock<ITasksRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteTaskCommandHandler(_tasksRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldDeleteTaskSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var deleteResult = Result.Success(taskId);

        _tasksRepositoryMock
            .Setup(repo => repo.DeleteById(taskId, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

        var command = new DeleteTaskCommand(taskId, ownerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(taskId);
    }

    [Fact]
    public async Task Handle_TaskNotFound_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var error = new Error("TaskNotFound", "Task not found or user is not authorized to delete this task.");
        var deleteResult = Result.Failure<Guid>(error);

        _tasksRepositoryMock
            .Setup(repo => repo.DeleteById(taskId, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new DeleteTaskCommand(taskId, ownerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TaskNotFound");
    }

    [Fact]
    public async Task Handle_TaskIdNotProvided_ShouldReturnFailure()
    {
        // Arrange
        var invalidTaskId = Guid.Empty; // Невірний id задачі
        var ownerId = Guid.NewGuid();

        var error = new Error("InvalidTaskId", "Task ID must be provided.");

        // У цьому випадку метод з репозиторія DeleteById взагалі не викликається
        _tasksRepositoryMock
            .Setup(repo => repo.DeleteById(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable(); // Перевірка ,що метод не був викликаний

        var command = new DeleteTaskCommand(invalidTaskId, ownerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("InvalidTaskId");

        // Переконуємось, що метод DeleteById не був викликаний
        _tasksRepositoryMock.Verify(repo => repo.DeleteById(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotAuthorized_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid(); // Незнайдений користувач

        var error = new Error("UserNotAuthorized", "User is not authorized to delete this task.");
        var deleteResult = Result.Failure<Guid>(error);

        _tasksRepositoryMock
            .Setup(repo => repo.DeleteById(taskId, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new DeleteTaskCommand(taskId, ownerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("UserNotAuthorized");
    }

    [Fact]
    public async Task Handle_OwnerIdNotProvided_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var invalidOwnerId = Guid.Empty; // Невалідний id Користувача

        var error = new Error("InvalidOwnerId", "Owner ID must be provided.");

        _tasksRepositoryMock
            .Setup(repo => repo.DeleteById(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        var command = new DeleteTaskCommand(taskId, invalidOwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("InvalidOwnerId");

        _tasksRepositoryMock.Verify(repo => repo.DeleteById(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_OwnerIdNotFound_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid(); // Введений id користувача не існує

        var error = new Error("OwnerNotFound", "Owner not found or user is not authorized to delete this task.");
        var deleteResult = Result.Failure<Guid>(error);

        _tasksRepositoryMock
            .Setup(repo => repo.DeleteById(taskId, ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deleteResult);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new DeleteTaskCommand(taskId, ownerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("OwnerNotFound");
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var expectedExceptionMessage = "Database error";

        _tasksRepositoryMock
            .Setup(repo => repo.DeleteById(taskId, ownerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        var command = new DeleteTaskCommand(taskId, ownerId);

        // Act
        Func<Task<Result<Guid>>> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage(expectedExceptionMessage);
    }
}
