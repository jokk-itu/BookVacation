using FluentValidation;

namespace Mediator.Tests.Handlers;

public class TestQueryValidator : AbstractValidator<TestQuery>
{
    public TestQueryValidator()
    {
        RuleFor(x => x.AboveZero).GreaterThan(0);
        RuleFor(x => x.BelowZero).LessThan(0);
    }
}