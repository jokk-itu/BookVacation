using System.Diagnostics;
using System.Net.Http.Json;
using Bogus;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;

namespace TestConsole;

public static class FlightLoad
{
    public static void Start()
    {
        var httpClient = new HttpClient();

        var airplaneRequestFaker = new Faker<PostAirplaneRequest>().Rules((faker, request) =>
        {
            request.Seats = 2;
            request.AirlineName = faker.Name.FirstName();
            request.ModelNumber = Guid.NewGuid();
            request.AirplaneMakerName = faker.Name.FirstName();
        });
        var flightRequestFaker = new Faker<PostFlightRequest>().Rules((faker, request) =>
        {
            request.Price = (decimal)Random.Shared.NextDouble();
            request.FromAirport = faker.Company.CompanyName();
            request.ToAirport = faker.Company.CompanyName();
            request.From = DateTimeOffset.Now.AddDays(Random.Shared.Next(1, 1000));
            request.To = request.From.AddDays(Random.Shared.Next(1, 10));
        });
        
        var airplane = Step.Create("post_airplane", async context =>
        {
            var airplaneRequest = airplaneRequestFaker.Generate();
            var watch = Stopwatch.StartNew();
            var airplaneResponse =
                await httpClient.PostAsJsonAsync("http://localhost:5001/api/v1/airplane", airplaneRequest);
            watch.Stop();
            airplaneResponse.EnsureSuccessStatusCode();
            var airplane = await airplaneResponse.Content.ReadFromJsonAsync<PostAirplaneResponse>();
            context.Data["airplane"] = airplane;
            var size = airplaneResponse.Content.Headers.ContentLength.GetValueOrDefault();
            return Response.Ok(statusCode: (int)airplaneResponse.StatusCode, sizeBytes: (int)size,
                latencyMs: watch.ElapsedMilliseconds);
        });

        var flight = Step.Create("post_flight", async context =>
        {
            var flightRequest = flightRequestFaker.Generate();
            flightRequest.AirPlaneId = (context.Data["airplane"] as PostAirplaneResponse)!.Id;
            var watch = Stopwatch.StartNew();
            var flightResponse = await httpClient.PostAsJsonAsync("http://localhost:5001/api/v1/flight", flightRequest);
            watch.Stop();
            flightResponse.EnsureSuccessStatusCode();
            var flight = await flightResponse.Content.ReadFromJsonAsync<PostFlightResponse>();
            context.Data["flight"] = flight;

            var size = flightResponse.Content.Headers.ContentLength.GetValueOrDefault();
            return Response.Ok(statusCode: (int)flightResponse.StatusCode, sizeBytes: (int)size,
                latencyMs: watch.ElapsedMilliseconds);
        });
        
        var scenario = ScenarioBuilder.CreateScenario("flight", airplane, flight);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("Flight")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}