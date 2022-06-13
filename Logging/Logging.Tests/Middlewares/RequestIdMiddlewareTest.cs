using Logging.Constants;
using Logging.Meta;
using Logging.Middlewares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Logging.Test.Middlewares;

public class RequestIdMiddlewareTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Invoke_ExpectRequestInMetaContext()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var requestId = Guid.NewGuid().ToString();
        httpContext.Request.Headers.Add(Header.RequestId, requestId);
        var metaContextAccessor = new MetaContextAccessor
        {
            MetaContext = new MetaContext()
        };
        Task Next(HttpContext _) => Task.CompletedTask;
        var requestIdMiddleware = new RequestIdMiddleware(Next);

        //Act
        await requestIdMiddleware.Invoke(httpContext, metaContextAccessor);

        //Assert
        Assert.Equal(requestId, metaContextAccessor.MetaContext!.RequestId);
    }
}