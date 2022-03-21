using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Raven.TestDriver;
using TrackingService.Domain;
using TrackingService.Infrastructure.Requests.ReadTracking;
using Xunit;

namespace TrackingService.Tests;

public class ReadTrackingRequestHandlerTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenNotExisting_ExpectNull()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();

        var request = new ReadTrackingRequest(Guid.NewGuid().ToString());
        var handler = new ReadTrackingRequestHandler(session);
        
        //Act
        var actual = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.Null(actual);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_GivenExisting_ExpectTracking()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();

        var tracking = new Tracking
        {
            Statuses = new List<Status>()
        };
        await session.StoreAsync(tracking);
        await session.SaveChangesAsync();
        WaitForIndexing(store);

        var request = new ReadTrackingRequest(tracking.Id);
        var handler = new ReadTrackingRequestHandler(session);
        
        //Act
        var actual = await handler.Handle(request, CancellationToken.None);

        //Assert
        Assert.NotNull(actual);
        Assert.Equal(tracking.Id, actual!.Id);
    }
}