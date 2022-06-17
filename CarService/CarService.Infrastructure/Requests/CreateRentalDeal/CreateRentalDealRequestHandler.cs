using CarService.Domain;
using DocumentClient;
using Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace CarService.Infrastructure.Requests.CreateRentalDeal;

public class CreateRentalDealRequestHandler : ICommandHandler<CreateRentalDealRequest, RentalDeal>
{
    private readonly IDocumentClient _client;
    private readonly ILogger<CreateRentalDealRequestHandler> _logger;

    public CreateRentalDealRequestHandler(IDocumentClient client, ILogger<CreateRentalDealRequestHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Response<RentalDeal>> Handle(CreateRentalDealRequest request, CancellationToken cancellationToken)
    {
        var rentalCar = await _client.QueryAsync<RentalCar>(query =>
            query.Where(x => x.Id == request.RentalCarId.ToString())
                .FirstOrDefaultAsync(cancellationToken)
        );

        if (rentalCar is null)
        {
            _logger.LogDebug("RentalCar does not exist from given identifier {Identifier}", request.RentalCarId);
            return new Response<RentalDeal>(ResponseCode.NotFound,
                new[] { "RentalCar does not exist from the given identifier" });
        }

        var conflictingRentDeal = await _client.QueryAsync<RentalDeal>(queryable =>
            queryable.Where(x => x.RentalCarId == request.RentalCarId)
                .Where(x =>
                    (request.RentFrom >= x.RentFrom && request.RentFrom <= x.RentTo) ||
                    (request.RentTo >= x.RentFrom && request.RentTo <= x.RentTo))
                .FirstOrDefaultAsync(cancellationToken)
        );


        if (conflictingRentDeal is not null)
        {
            _logger.LogDebug("RentalCar with identifier {Identifier}, is already booked from {From} to {To}",
                request.RentalCarId, request.RentFrom, request.RentTo);
            return new Response<RentalDeal>(ResponseCode.Conflict,
                new[] { "RentalDeal already exists in the given timeframe." });
        }

        var rentalDeal = new RentalDeal
        {
            RentFrom = request.RentFrom,
            RentTo = request.RentTo,
            RentalCarId = request.RentalCarId
        };

        await _client.StoreAsync(rentalDeal, cancellationToken);
        return new Response<RentalDeal>(rentalDeal);
    }
}