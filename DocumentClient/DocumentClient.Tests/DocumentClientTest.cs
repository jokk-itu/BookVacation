using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.Client.Documents;
using Raven.TestDriver;
using Xunit;

namespace DocumentClient.Tests;

public class DocumentClientTest : RavenTestDriver
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteAsync_GivenExistingKey_DeleteSuccess()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());

        var document = new DummyDocument();
        await client.StoreAsync(document);
        WaitForIndexing(store);

        //Act
        var isDeleted = await client.DeleteAsync(document.Id, CancellationToken.None);

        //Assert
        Assert.True(isDeleted);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteAsync_GivenNonExistingKey_DeleteUnsuccessful()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());

        //Act
        var isDeleted = await client.DeleteAsync(Guid.NewGuid().ToString(), CancellationToken.None);

        //Assert
        Assert.False(isDeleted);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task StoreAsync_GivenNonExistingDocument_StoreSuccessfully()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());

        //Act
        var isStored = await client.StoreAsync(new DummyDocument());

        //Assert
        Assert.True(isStored);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task StoreAsync_GivenExistingDocument_StoreUnsuccessfully()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());
        var document = new DummyDocument();
        await client.StoreAsync(document);
        WaitForIndexing(store);

        //Act
        var isStored = await client.StoreAsync(new DummyDocument { Id = document.Id });

        //Assert
        Assert.False(isStored);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ExistsAsync_GivenExistingDocument_ReturnsTrue()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());
        var document = new DummyDocument();
        await client.StoreAsync(document);
        WaitForIndexing(store);

        //Act
        var exists = await client.ExistsAsync(document.Id);

        //Assert
        Assert.True(exists);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task ExistsAsync_GivenNonExistingDocument_ReturnsFalse()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());

        //Act
        var exists = await client.ExistsAsync(Guid.NewGuid().ToString());

        //Assert
        Assert.False(exists);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoadSingleAsync_GivenExistingDocument_ReturnsDocument()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());
        var document = new DummyDocument();
        await client.StoreAsync(document);
        WaitForIndexing(store);

        //Act
        var loadedDocument = await client.LoadSingleAsync<DummyDocument>(document.Id);

        //Assert
        Assert.NotNull(loadedDocument);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoadSingleAsync_GivenNonExistingDocument_ReturnsNull()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());

        //Act
        var loadedDocument = await client.LoadSingleAsync<DummyDocument>(Guid.NewGuid().ToString());

        //Assert
        Assert.Null(loadedDocument);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoadAsync_GivenExistingDocuments_ReturnsDocuments()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());
        var document1 = new DummyDocument();
        var document2 = new DummyDocument();
        await client.StoreAsync(document1);
        await client.StoreAsync(document2);
        WaitForIndexing(store);

        //Act
        var documents = await client.LoadAsync<DummyDocument>(new[] { document1.Id, document2.Id });

        //Assert
        Assert.True(documents.ContainsKey(document1.Id));
        Assert.True(documents.ContainsKey(document2.Id));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoadAsync_GivenSomeExistingDocuments_ReturnsSomeDocuments()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());
        var document1 = new DummyDocument();
        var document2 = new DummyDocument();
        await client.StoreAsync(document1);
        await client.StoreAsync(document2);
        WaitForIndexing(store);

        //Act
        var fakeId1 = Guid.NewGuid().ToString();
        var fakeId2 = Guid.NewGuid().ToString();
        var documents = await client.LoadAsync<DummyDocument>(new[] { document1.Id, document2.Id, fakeId1, fakeId2 });

        //Assert
        Assert.True(documents.ContainsKey(document1.Id));
        Assert.True(documents.ContainsKey(document2.Id));
        Assert.Null(documents[fakeId1]);
        Assert.Null(documents[fakeId2]);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoadAsync_GivenNonExistingDocuments_ReturnsEmpty()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());

        //Act
        var fakeId1 = Guid.NewGuid().ToString();
        var fakeId2 = Guid.NewGuid().ToString();
        var documents = await client.LoadAsync<DummyDocument>(new[] { fakeId1, fakeId2 });

        //Assert
        Assert.Null(documents[fakeId1]);
        Assert.Null(documents[fakeId2]);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task QueryAsync_GivenExistingDocument_ReturnsDocument()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());
        var document = new DummyDocument();
        await client.StoreAsync(document);
        WaitForIndexing(store);

        //Act
        var queriedDocument = await client.QueryAsync<DummyDocument>(async query =>
            await query.Where(d => d.Id.Equals(document.Id)).FirstOrDefaultAsync());

        //Assert
        Assert.NotNull(queriedDocument);
        Assert.Equal(queriedDocument!.Id, document.Id);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task QueryAsync_GivenNonExistingDocument_ReturnsNull()
    {
        //Arrange
        using var store = GetDocumentStore();
        using var session = store.OpenAsyncSession();
        var client = new DocumentClient(session, Mock.Of<ILogger<DocumentClient>>());
        var id = Guid.NewGuid().ToString();

        //Act
        var queriedDocument = await client.QueryAsync<DummyDocument>(async query =>
            await query.Where(d => d.Id.Equals(id)).FirstOrDefaultAsync());

        //Assert
        Assert.Null(queriedDocument);
    }
}