using System.Net;
using Logging.DelegatingHandlers;
using Moq;
using Moq.Protected;
using Xunit;

namespace Logging.Test;

public class RequestIdDelegatingHandlerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Send_ExpectRequestIdHeader()
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");
        var innerHandlerMock = new Mock<DelegatingHandler>(MockBehavior.Strict);
        innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
            .Verifiable();
        
        var handler = new RequestIdDelegatingHandler
        {
            InnerHandler = innerHandlerMock.Object
        };
        var invoker = new HttpMessageInvoker(handler);

        //Act
        await invoker.SendAsync(request, CancellationToken.None);

        //Assert
        innerHandlerMock.Verify();
        Assert.True(request.Headers.TryGetValues("X-Request-Id", out var correlationId));
    }
}