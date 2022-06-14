using System.Net;
using FluentAssertions;
using Logging.DelegatingHandlers;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using Xunit;

namespace Logging.Test.DelegatingHandlers;

public class PerformanceDelegatingHandlerTest
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Send_ExpectStatusOk()
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");
        var innerHandlerMock = new Mock<DelegatingHandler>(MockBehavior.Strict);
        innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
            .Verifiable();
        Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddSerilog(Log.Logger);

        var handler = new PerformanceDelegatingHandler(loggerFactory.CreateLogger<PerformanceDelegatingHandler>())
        {
            InnerHandler = innerHandlerMock.Object
        };
        var invoker = new HttpMessageInvoker(handler);
        
        using (TestCorrelator.CreateContext())
        {
            //Act
            await invoker.SendAsync(request, CancellationToken.None);

            //Assert
            Assert.True(TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle().Which.Level ==
                        LogEventLevel.Information);
            innerHandlerMock.Verify();
        }
    }
    
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Send_ExpectStatusError()
    {
        //Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com");
        var innerHandlerMock = new Mock<DelegatingHandler>(MockBehavior.Strict);
        innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
            .Verifiable();
        Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
        var loggerFactory = new LoggerFactory();
        loggerFactory.AddSerilog(Log.Logger);

        var handler = new PerformanceDelegatingHandler(loggerFactory.CreateLogger<PerformanceDelegatingHandler>())
        {
            InnerHandler = innerHandlerMock.Object
        };
        var invoker = new HttpMessageInvoker(handler);
        
        using (TestCorrelator.CreateContext())
        {
            //Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await invoker.SendAsync(request, CancellationToken.None));

            //Assert
            Assert.True(TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle().Which.Level ==
                        LogEventLevel.Error);
            innerHandlerMock.Verify();
        }
    }
}