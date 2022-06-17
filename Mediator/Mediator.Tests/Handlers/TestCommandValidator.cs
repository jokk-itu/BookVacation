using FluentValidation;

namespace Mediator.Tests.Handlers;

public class TestCommandValidator : AbstractValidator<TestCommand>
{
    public TestCommandValidator()
    {
        RuleFor(x => x.AboveZero).GreaterThan(0);
        RuleFor(x => x.BelowZero).LessThan(0);
    }
}