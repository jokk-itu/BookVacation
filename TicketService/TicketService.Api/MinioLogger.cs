using Minio;
using Minio.DataModel.Tracing;

namespace TicketService.Api;

public class MinioLogger : IRequestLogger
{
    private readonly ILogger<MinioLogger> _logger;

    public MinioLogger(ILogger<MinioLogger> logger)
    {
        _logger = logger;
    }

    public void LogRequest(RequestToLog requestToLog, ResponseToLog responseToLog, double durationMs)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   { "Uri", requestToLog.uri }, { "Resource", requestToLog.resource }, { "Method", requestToLog.method }
               }))
        {
            _logger.LogDebug("Request started");

            if (string.IsNullOrWhiteSpace(responseToLog.errorMessage))
                _logger.LogInformation("Request completed with statuscode {StatusCode}, took {Elapsed} ms",
                    responseToLog.statusCode, responseToLog.durationMs);
            else
                _logger.LogError(
                    "Request completed with statuscode {StatusCode}, took {Elapsed} ms, with error {Error}",
                    responseToLog.statusCode, responseToLog.durationMs, responseToLog.errorMessage);
        }
    }
}