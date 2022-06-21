using DocumentClient;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarService.Infrastructure.Requests.DeleteRentalDeal;

public class DeleteRentalDealCommandHandler : ICommandHandler<DeleteRentalDealCommand, Unit>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<DeleteRentalDealCommandHandler> _logger;

    public DeleteRentalDealCommandHandler(IDocumentClient client, ILogger<DeleteRentalDealCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<Unit>> Handle(DeleteRentalDealCommand command, CancellationToken cancellationToken)
    {
        var isDeleted = await _client.DeleteAsync(command.RentalDealId.ToString(), cancellationToken);

        if (isDeleted) return new Response<Unit>();
        
        _logger.LogDebug("RentalDeal does not exist from given identifier {}", command.RentalDealId);
        return new Response<Unit>
        {
            ResponseCode = ResponseCode.NotFound
        };

    }
}