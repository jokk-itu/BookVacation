using MediatR;
using Neo4j.Driver;

namespace HotelService.Requests.CreateRentHotel;

public class CreateRentHotelRequestHandler : IRequestHandler<CreateRentHotelRequest, RequestResult>
{
    private readonly IDriver _driver;

    public CreateRentHotelRequestHandler(IDriver driver)
    {
        _driver = driver;
    }
    
    public async Task<RequestResult> Handle(CreateRentHotelRequest request, CancellationToken cancellationToken)
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
            var isSuccessful = await result.FetchAsync();
            if (isSuccessful)
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