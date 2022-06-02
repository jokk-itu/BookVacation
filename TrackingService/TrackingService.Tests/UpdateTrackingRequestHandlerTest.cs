using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
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

        var request = new UpdateTrackingRequest(tracking.Id, "Success", DateTimeOffset.Now);
        var handler = new UpdateTrackingRequestHandler(client);

        //Act
        var actual = await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);
        var expected = await session.Query<Tracking>().Where(x => x.Id == actual.Id).FirstOrDefaultAsync();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_TrackingNotExists()
    {
        //Arrange
        var store = GetDocumentStore();
        var session = store.OpenAsyncSession();
        var client = new DocumentClient.DocumentClient(session, Mock.Of<ILogger<DocumentClient.DocumentClient>>());
        var request = new UpdateTrackingRequest(Guid.NewGuid().ToString(), "Success", DateTimeOffset.Now);
        var handler = new UpdateTrackingRequestHandler(client);

        //Act
        var actual = await handler.Handle(request, CancellationToken.None);
        WaitForIndexing(store);
        var expected = await session.Query<Tracking>().Where(x => x.Id == actual.Id).FirstOrDefaultAsync();

        //Assert
        Assert.Equal(expected, actual);
    }
}