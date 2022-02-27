namespace TicketService.Domain.Templates.HotelTicket;

public record HotelTicketModel(Guid HotelId, Guid RoomId)
{
    public async Task<string> RenderViewAsync(CancellationToken cancellationToken = default)
    {
        return await HotelTicket.RenderAsync(this, cancellationToken);
    }
}