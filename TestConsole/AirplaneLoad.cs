using System.Diagnostics;
using System.Net.Http.Json;
using Bogus;
using FlightService.Contracts.Airplane;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;

namespace TestConsole;

public static class AirplaneLoad
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
        
        var scenario = ScenarioBuilder.CreateScenario("airplane", airplane);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("Airplane")
            .WithTestSuite("Airplane")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}