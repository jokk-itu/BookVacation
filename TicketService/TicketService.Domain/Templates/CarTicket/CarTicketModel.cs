namespace TicketService.Domain.Templates.CarTicket;

public record CarTicketModel(Guid CarId, Guid RentingCompanyId)
{
    public async Task<string> RenderViewAsync(CancellationToken cancellationToken = default)
    {
        return await CarTicket.RenderAsync(this, cancellationToken);
    }
}