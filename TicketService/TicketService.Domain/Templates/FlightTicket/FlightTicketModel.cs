namespace TicketService.Domain.Templates.FlightTicket;

public record FlightTicketModel(Guid FlightId)
{
    public async Task<string> RenderViewAsync(CancellationToken cancellationToken = default)
    {
        return await FlightTicket.RenderAsync(this, cancellationToken);
    }
}