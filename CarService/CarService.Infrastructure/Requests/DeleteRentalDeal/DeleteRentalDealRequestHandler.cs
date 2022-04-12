using DocumentClient;
using MediatR;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public class DeleteRentalDealRequestHandler : IRequestHandler<DeleteRentalDealRequest, Unit>
{
    private readonly IDocumentClient _client;

    public DeleteRentalDealRequestHandler(IDocumentClient client)
    {
        _client = client;
    }

    public async Task<Unit> Handle(DeleteRentalDealRequest request, CancellationToken cancellationToken)
    {
        await _client.DeleteAsync(request.RentalDealId.ToString(), cancellationToken);
        return Unit.Value;
    }
}