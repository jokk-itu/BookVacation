using Logging.Constants;
using Logging.Middlewares;
using Meta;
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
        var correlationId = Guid.NewGuid();
        httpContext.Request.Headers.Add(Header.CorrelationId, correlationId.ToString());
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