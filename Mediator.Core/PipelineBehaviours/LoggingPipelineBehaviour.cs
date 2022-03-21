using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mediator.PipelineBehaviours;

public class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger;

    public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogDebug("Handling {Request}", typeof(TRequest).Name);
        var watch = Stopwatch.StartNew();
        var response = await next();
        watch.Stop();
        _logger.LogInformation("Handled {Request}, returned {Response}, took {Elapsed}", typeof(TRequest).Name,
            typeof(TResponse).Name, watch.ElapsedMilliseconds);
        return response;
    }
}