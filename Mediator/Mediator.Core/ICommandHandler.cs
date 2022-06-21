using MediatR;

namespace Mediator;

public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, Response<TResponse>>
    where TRequest : ICommand<TResponse>
{
}