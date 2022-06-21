using Bogus;
using FlightService.Contracts.Airplane;

namespace Core.Fakes;

public static class AirplaneFaker
{
    public static Faker<PostAirplaneRequest> GetAirplaneRequestFaker()
    {
        return new Faker<PostAirplaneRequest>().Rules((faker, request) =>
        {
            request.Seats = 2;
            request.AirlineName = faker.Name.FirstName();
            request.ModelNumber = Guid.NewGuid();
            request.AirplaneMakerName = faker.Name.FirstName();
        });
    }
}