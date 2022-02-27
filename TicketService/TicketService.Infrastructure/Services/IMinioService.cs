namespace TicketService.Infrastructure.Services;

public interface IMinioService
{
    Task<Stream?> FetchTicketAsync(string bucket, string ticketId, CancellationToken cancellationToken = default);
    Task<bool> PutTicketAsync(string bucket, string ticketId, byte[] data, CancellationToken cancellationToken = default);
}