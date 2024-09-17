using FluentAssertions;
using ManagmentSystem.Application.Abstractions.Data;
using ManagmentSystem.Application.Tasks.Commands.CreateTask;
using ManagmentSystem.Application.Tasks.Commands.UpdateTask;
using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Abstractions;
using Moq;
using TaskStatus = ManagmentSystem.Core.Enums.TaskStatus;

namespace ManagmentSystem.Application.UnitTests.Tasks.Commands;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITasksRepository> _tasksRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _tasksRepositoryMock = new Mock<ITasksRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateTaskCommandHandler(_tasksRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldUpdateTaskAndReturnSuccess()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateTaskCommand(
            taskId,
            "Updated Title",
            "Updated Description",
            DateTime.UtcNow.AddDays(1),
            TaskStatus.InProgress,
            TaskPriority.Medium,
            userId
        );

        // Мокаємо метод оновлення завдання
        _tasksRepositoryMock
            .Setup(repo => repo.UpdateById(
                taskId,
                "Updated Title",
                "Updated Description",
                It.IsAny<DateTime>(),
                TaskStatus.InProgress,
                TaskPriority.Medium,
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(taskId));

        // Мокаємо метод збереження змін у UnitOfWork
        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

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
        var userId = Guid.NewGuid();
        var command = new UpdateTaskCommand(
            taskId,
            "Updated Title",
            "Updated Description",
            DateTime.UtcNow.AddDays(1),
            TaskStatus.InProgress,
            TaskPriority.Medium,
            userId
        );

        var error = new Error("TaskNotFound", "Task not found or user is not authorized to update this task.");
        var updateResult = Result.Failure<Guid>(error);

        // Мокаємо метод оновлення завдання
        _tasksRepositoryMock
            .Setup(repo => repo.UpdateById(
                taskId,
                "Updated Title",
                "Updated Description",
                It.IsAny<DateTime>(),
                TaskStatus.InProgress,
                TaskPriority.Medium,
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TaskNotFound");
    }

    [Fact]
    public async Task Handle_InvalidTitle_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateTaskCommand(
            taskId,
            new string('A', Core.Models.Task.MAX_TITLE_LENGTH + 1), // Довгий заголовок
            "Valid Description",
            DateTime.UtcNow.AddDays(1),
            TaskStatus.InProgress,
            TaskPriority.Medium,
            userId
        );

        // Встановлення помилки валідації в репизиторії
        var error = new Error("InvalidTitle", $"Title cannot be empty or longer than {Core.Models.Task.MAX_TITLE_LENGTH} characters.");
        var updateResult = Result.Failure<Guid>(error);

        _tasksRepositoryMock
            .Setup(repo => repo.UpdateById(
                taskId,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<TaskStatus>(),
                It.IsAny<TaskPriority>(),
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updateResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("InvalidTitle");
    }

    [Fact]
    public async Task Handle_EmptyUserId_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskCommand(
            taskId,
            "Valid Title",
            "Valid Description",
            DateTime.UtcNow.AddDays(1),
            TaskStatus.InProgress,
            TaskPriority.Medium,
            Guid.Empty // Порожній UserId
        );

        var error = new Error("InvalidUserId", "User ID must be provided.");

        // Мокаємо метод оновлення завдання
        _tasksRepositoryMock
            .Setup(repo => repo.UpdateById(
                taskId,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<TaskStatus>(),
                It.IsAny<TaskPriority>(),
                Guid.Empty,
                It.IsAny<CancellationToken>()))
            .Verifiable(); // Перевіряємо, що метод з репозиторію не викликається

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("InvalidUserId");

        // Переконуємось, що метод UpdateById не був викликаний
        _tasksRepositoryMock.Verify(repo => repo.UpdateById(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<TaskStatus>(), It.IsAny<TaskPriority>(), Guid.Empty, It.IsAny<CancellationToken>()), Times.Never);
    }
}
