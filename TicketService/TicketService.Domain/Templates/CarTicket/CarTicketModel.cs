namespace TicketService.Domain.Templates.CarTicket;

public record CarTicketModel(Guid CarId, string RentingCompanyName)
{
    public async Task<string> RenderViewAsync(CancellationToken cancellationToken = default)
    {
        return await CarTicket.RenderAsync(this, cancellationToken);
    }
}