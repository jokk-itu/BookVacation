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
        _logger.LogDebug("Request started");
        _logger.LogInformation("Request completed");
    }
}