namespace TicketService.Infrastructure.Services;

public interface IMinioService
{
    Task<Stream?> FetchTicketAsync(string bucket, string ticketId, CancellationToken cancellationToken = default);
    Task<bool> PutTicketAsync(string bucket, string ticketId, Stream data, CancellationToken cancellationToken = default);
}