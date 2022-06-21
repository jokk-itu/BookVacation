using CarService.Domain;
using DocumentClient;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace CarService.Infrastructure.Requests.CreateRentalDeal;

public class CreateRentalDealCommandHandler : ICommandHandler<CreateRentalDealCommand, RentalDeal>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<CreateRentalDealCommandHandler> _logger;

    public CreateRentalDealCommandHandler(IDocumentClient client, ILogger<CreateRentalDealCommandHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<RentalDeal>> Handle(CreateRentalDealCommand command, CancellationToken cancellationToken)
    {
        var rentalCar = await _client.QueryAsync<RentalCar>(query =>
            query.Where(x => x.Id == command.RentalCarId.ToString())
                .FirstOrDefaultAsync(cancellationToken)
        );

        if (rentalCar is null)
        {
            _logger.LogDebug("RentalCar does not exist from given identifier {}", command.RentalCarId);
            return new Response<RentalDeal>(ResponseCode.NotFound,
                new[] { "RentalCar does not exist from the given identifier" });
        }

        var conflictingRentDeal = await _client.QueryAsync<RentalDeal>(queryable =>
            queryable.Where(x => x.RentalCarId == command.RentalCarId)
                .Where(x =>
                    (command.RentFrom >= x.RentFrom && command.RentFrom <= x.RentTo) ||
                    (command.RentTo >= x.RentFrom && command.RentTo <= x.RentTo))
                .FirstOrDefaultAsync(cancellationToken)
        );


        if (conflictingRentDeal is not null)
        {
            _logger.LogDebug("RentalCar with identifier {}, is already booked from {From} to {To}",
                command.RentalCarId, command.RentFrom, command.RentTo);
            return new Response<RentalDeal>(ResponseCode.Conflict,
                new[] { "RentalDeal already exists in the given timeframe" });
        }

        var rentalDeal = new RentalDeal
        {
            RentFrom = command.RentFrom,
            RentTo = command.RentTo,
            RentalCarId = command.RentalCarId
        };

        await _client.StoreAsync(rentalDeal, cancellationToken);
        return new Response<RentalDeal>(rentalDeal);
    }
}