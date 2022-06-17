using DocumentClient;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public class DeleteRentalDealRequestHandler : ICommandHandler<DeleteRentalDealRequest, Unit>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<DeleteRentalDealRequestHandler> _logger;

    public DeleteRentalDealRequestHandler(IDocumentClient client, ILogger<DeleteRentalDealRequestHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<Unit>> Handle(DeleteRentalDealRequest request, CancellationToken cancellationToken)
    {
        var deleted = await _client.DeleteAsync(request.RentalDealId.ToString(), cancellationToken);

        if (deleted) return new Response<Unit>();
        
        _logger.LogDebug("RentalDeal does not exist from given identifier {Identifier}", request.RentalDealId);
        return new Response<Unit>
        {
            ResponseCode = ResponseCode.NotFound
        };

    }
}