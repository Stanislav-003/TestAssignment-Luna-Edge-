﻿using ManagmentSystem.Core.Shared;
using MediatR;

namespace ManagmentSystem.Application.Abstractions.Messaging
{
    public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    {
    }
}
