namespace Mediator.Tests.Handlers;

public record TestQuery(int BelowZero, int AboveZero) : IQuery<int>;