using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Tracing;

namespace BlobStorage;

public class RequestLogger : IRequestLogger
{
    private readonly ILogger<RequestLogger> _logger;

    public RequestLogger(ILogger<RequestLogger> logger)
    {
        _logger = logger;
    }

    public void LogRequest(RequestToLog requestToLog, ResponseToLog responseToLog, double durationMs)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   { "Uri", requestToLog.uri }, { "Method", requestToLog.method }
               }))
        {
            _logger.LogDebug("Request started");

            if (string.IsNullOrWhiteSpace(responseToLog.errorMessage))
                _logger.LogInformation(
                    "Request to {Resource} completed with statuscode {StatusCode}, took {Elapsed} ms",
                    requestToLog.resource, responseToLog.statusCode, responseToLog.durationMs);
            else
                _logger.LogError(
                    "Request to {Resource} completed with statuscode {StatusCode}, took {Elapsed} ms, with error {Error}",
                    requestToLog.resource, responseToLog.statusCode, responseToLog.durationMs,
                    responseToLog.errorMessage);
        }
    }
}