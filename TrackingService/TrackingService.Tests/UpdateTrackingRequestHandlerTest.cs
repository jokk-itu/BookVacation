using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.TestDriver;
using TrackingService.Domain;
using TrackingService.Infrastructure.Requests.UpdateTracking;
using Xunit;

namespace TrackingService.Tests;

public class UpdateTrackingRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_TrackingExists()
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

        var request = new UpdateTrackingCommand(tracking.Id, "Success", DateTimeOffset.Now);
        var handler = new UpdateTrackingCommandHandler(client);

        //Act
        var trackingResponse = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Ok, trackingResponse.ResponseCode);
        Assert.NotNull(trackingResponse.Body);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_TrackingNotExists()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var request = new UpdateTrackingCommand(Guid.NewGuid().ToString(), "Success", DateTimeOffset.Now);
        var handler = new UpdateTrackingCommandHandler(client);

        //Act
        var trackingResponse = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Equal(ResponseCode.Ok, trackingResponse.ResponseCode);
        Assert.NotNull(trackingResponse.Body);
    }
}