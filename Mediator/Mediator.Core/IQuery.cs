using MediatR;

namespace Mediator;

public interface IQuery<T> : IRequest<Response<T>> {}