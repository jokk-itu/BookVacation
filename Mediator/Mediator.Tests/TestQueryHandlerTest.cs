using FluentValidation;
using Mediator.Tests.Handlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mediator.Tests;

public class TestQueryHandlerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectResponseOk()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestQueryHandler).Assembly);
        services.AddValidatorsFromAssembly(typeof(TestQueryValidator).Assembly);
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();

        //Act
        var query = new TestQuery(-1, 1);
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var response = await mediator.Send(query);

        //Assert
        Assert.Equal(ResponseCode.Ok, response.ResponseCode);
    }
    
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectThrowValidationException()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddMediator(typeof(TestQueryHandler).Assembly);
        services.AddValidatorsFromAssemblyContaining<TestQueryValidator>();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();

        //Act
        var query = new TestQuery(1, -1);
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        //Assert
        await Assert.ThrowsAsync<ValidationException>(async () => await mediator.Send(query));
    }
}