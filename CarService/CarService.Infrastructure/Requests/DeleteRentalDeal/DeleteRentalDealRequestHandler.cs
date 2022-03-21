using MediatR;
using Raven.Client.Documents.Session;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public class DeleteRentalDealRequestHandler : IRequestHandler<DeleteRentalDealRequest, Unit>
{
    private readonly IAsyncDocumentSession _session;

    public DeleteRentalDealRequestHandler(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Unit> Handle(DeleteRentalDealRequest request, CancellationToken cancellationToken)
    {
        _session.Delete(request.RentalDealId);
        await _session.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}