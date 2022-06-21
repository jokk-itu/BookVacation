using MediatR;

namespace Mediator;

public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, Response<TResponse>>
    where TRequest : IQuery<TResponse>
{
}