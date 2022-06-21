namespace Mediator.Tests.Handlers;

public record TestCommand(int BelowZero, int AboveZero) : ICommand<int>;