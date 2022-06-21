using MassTransit.Testing;
using VacationService.Infrastructure.Requests.CreateVacation;
using Xunit;

namespace VacationService.Test;

public class CreateVacationRequestHandlerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectSentRoutingSlip()
    {
        //Arrange
        var harness = new InMemoryTestHarness();
        var request = new CreateVacationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(), 
            Guid.NewGuid(),
            DateTimeOffset.Now,
            DateTimeOffset.Now, 
            Guid.NewGuid(),
            Guid.NewGuid(),
            string.Empty,
            DateTimeOffset.Now,
            DateTimeOffset.Now);

        //Act
        await harness.Start();
        try
        {
            var requestHandler = new CreateVacationRequestHandler(harness.BusControl);
            await requestHandler.Handle(request, CancellationToken.None);
            
            //Assert
            Assert.True(await harness.Sent.Any());
        }
        finally
        {
            await harness.Stop();
        }
    }
}