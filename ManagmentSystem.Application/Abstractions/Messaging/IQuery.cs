using ManagmentSystem.Core.Shared;
using MediatR;

namespace ManagmentSystem.Application.Abstractions.Messaging
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
