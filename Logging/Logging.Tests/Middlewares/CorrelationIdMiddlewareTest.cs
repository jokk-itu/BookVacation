using Logging.Constants;
using Logging.Meta;
using Logging.Middlewares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Logging.Test.Middlewares;

public class CorrelationIdMiddlewareTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Invoke_ExpectCorrelationIdInMetaContext()
    {
        //Arrange
        var httpContext = new DefaultHttpContext();
        var correlationId = Guid.NewGuid().ToString();
        httpContext.Request.Headers.Add(Header.CorrelationId, correlationId);
        var metaContextAccessor = new MetaContextAccessor
        {
            MetaContext = new MetaContext()
        };
        Task Next(HttpContext _) => Task.CompletedTask;
        var correlationIdMiddleware = new CorrelationIdMiddleware(Next);

        //Act
        await correlationIdMiddleware.Invoke(httpContext, metaContextAccessor);

        //Assert
        Assert.Equal(correlationId, metaContextAccessor.MetaContext!.CorrelationId);
    }
}