using System.Net;
using Logging.DelegatingHandlers;
using Logging.Meta;
using Moq;
using Moq.Protected;
using Xunit;

namespace Logging.Test;

public class CorrelationIdDelegatingHandlerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Send_ExpectCorrelationIdHeader()
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");
        var innerHandlerMock = new Mock<DelegatingHandler>(MockBehavior.Strict);
        innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
            .Verifiable();
        var expected = Guid.NewGuid().ToString();
        var metaContextAccessor = new MetaContextAccessor
        {
            MetaContext = new MetaContext
            {
                CorrelationId = expected,
                RequestId = Guid.NewGuid().ToString()
            }
        };
        
        var handler = new CorrelationIdDelegatingHandler(metaContextAccessor)
        {
            InnerHandler = innerHandlerMock.Object
        };
        var invoker = new HttpMessageInvoker(handler);

        //Act
        await invoker.SendAsync(request, CancellationToken.None);

        //Assert
        innerHandlerMock.Verify();
        Assert.True(request.Headers.TryGetValues("X-Correlation-Id", out var actual));
        Assert.Equal(expected, actual!.Single());
    }
}