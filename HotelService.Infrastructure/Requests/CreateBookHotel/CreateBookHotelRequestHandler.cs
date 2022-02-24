using MediatR;
using Neo4j.Driver;

namespace HotelService.Infrastructure.Requests.CreateBookHotel;

public class CreateBookHotelRequestHandler : IRequestHandler<CreateBookHotelRequest, RequestResult>
{
    private readonly IDriver _driver;

    public CreateBookHotelRequestHandler(IDriver driver)
    {
        _driver = driver;
    }

    public async Task<RequestResult> Handle(CreateBookHotelRequest request, CancellationToken cancellationToken)
    {
        await using var session = _driver.AsyncSession();
        var isSuccessful = await session.WriteTransactionAsync(async transaction =>
        {
            const string command = @"
MATCH (h:Hotel {id: $hotelId})-[:Has]->(r:Room {id: $roomId})
WHERE NOT EXISTS {
    MATCH 
        (:RentHotel)-->(h),
        (:RentHotel)-->(r)
}
CREATE (r1:RentHotel {id: $rentId})-[:At]->(h)
CREATE (r1)-[:Renting {days: $days}]->(r)
";
            var result = await transaction.RunAsync(command, new
            {
                days = request.Days,
                roomId = request.RoomId.ToString(),
                hotelId = request.HotelId.ToString(),
                rentId = request.RentId.ToString()
            });
            
            if (await result.FetchAsync())
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;
        });
        return isSuccessful
            ? RequestResult.Ok
            : RequestResult.Error;
    }
}