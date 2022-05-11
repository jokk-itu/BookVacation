using Bogus;
using FlightService.Contracts.Flight;

namespace TestConsole.Fakes;

public static class FlightFaker
{
    public static Faker<PostFlightRequest> GetFlightRequestFaker()
    {
        return new Faker<PostFlightRequest>().Rules((faker, request) =>
        {
            request.Price = (decimal)Random.Shared.NextDouble();
            request.FromAirport = faker.Company.CompanyName();
            request.ToAirport = faker.Company.CompanyName();
            request.From = DateTimeOffset.Now.AddDays(Random.Shared.Next(1, 1000));
            request.To = request.From.AddDays(Random.Shared.Next(1, 10));
        });
    }
}