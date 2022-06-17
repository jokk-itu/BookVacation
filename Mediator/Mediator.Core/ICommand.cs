using MediatR;

namespace Mediator;

public interface ICommand<T> : IRequest<Response<T>>
{
    
}