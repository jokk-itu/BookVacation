using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.TestDriver;
using TrackingService.Domain;
using TrackingService.Infrastructure.Requests.ReadTracking;
using Xunit;

namespace TrackingService.Tests;

public class ReadTrackingRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenNotExisting_ExpectNotFound()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var request = new ReadTrackingCommand(Guid.NewGuid().ToString());
        var handler = new ReadTrackingCommandHandler(client, Mock.Of<ILogger<ReadTrackingCommandHandler>>());

        //Act
        var trackingResponse = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.NotFound, trackingResponse.ResponseCode);
        Assert.Null(trackingResponse.Body);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenExisting_ExpectOk()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var tracking = new Tracking
        {
            Statuses = new List<Status>()
        };
        await session.StoreAsync(tracking);
        await session.SaveChangesAsync();
        WaitForIndexing(store);

        var request = new ReadTrackingCommand(tracking.Id);
        var handler = new ReadTrackingCommandHandler(client, Mock.Of<ILogger<ReadTrackingCommandHandler>>());

        //Act
        var trackingResponse = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Ok, trackingResponse.ResponseCode);
        Assert.NotNull(trackingResponse.Body);
    }
}