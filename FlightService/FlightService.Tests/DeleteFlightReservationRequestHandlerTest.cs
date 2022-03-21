using System.Threading.Tasks;
using Raven.TestDriver;
using Xunit;

namespace FlightService.Tests;

public class DeleteFlightReservationRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectOk()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();

        //Act

        //Assert
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExpectError()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();

        //Act

        //Assert
    }
}