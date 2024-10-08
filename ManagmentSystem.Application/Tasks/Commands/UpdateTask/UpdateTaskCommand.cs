﻿using Azure.Core;
using ManagmentSystem.Core.Enums;
using ManagmentSystem.Core.Shared;
using ManagmentSystem.DataAccess.Entities;
using MediatR;
using TaskStatus = ManagmentSystem.Core.Enums.TaskStatus;

namespace ManagmentSystem.Application.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string Description,
    DateTime DueDate,
    TaskStatus Status,
    TaskPriority Priority,
    Guid UserId) : IRequest<Result<Guid>>;