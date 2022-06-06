using Bogus;
using HotelService.Contracts.CreateHotel;

namespace TestConsole.Fakes;

public static class HotelFaker
{
    public static Faker<PostHotelRequest> GetFlightRequestFaker()
    {
        return new Faker<PostHotelRequest>().Rules((faker, request) =>
        {
            request.Rooms = 2;
            request.Address = faker.Address.StreetAddress();
            request.City = faker.Address.City();
            request.Country = faker.Address.Country();
        });
    }
}