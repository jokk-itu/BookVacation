using System.Diagnostics;
using System.Net.Http.Json;
using Bogus;
using HotelService.Contracts.CreateHotel;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;

namespace TestConsole;

public static class HotelLoad
{
    public static void Start()
    {
        var httpClient = new HttpClient();
        
        var hotelRequestFaker = new Faker<PostHotelRequest>().Rules((faker, request) =>
        {
            request.Rooms = 2;
            request.Address = faker.Address.StreetAddress();
            request.City = faker.Address.City();
            request.Country = faker.Address.Country();
        });
        
        var hotel = Step.Create("post_hotel", async context =>
        {
            var hotelRequest = hotelRequestFaker.Generate();
            var watch = Stopwatch.StartNew();
            var hotelResponse = await httpClient.PostAsJsonAsync("http://localhost:5002/api/v1/hotel", hotelRequest);
            watch.Stop();
            hotelResponse.EnsureSuccessStatusCode();
            var hotel = await hotelResponse.Content.ReadFromJsonAsync<PostHotelResponse>();
            context.Data["hotel"] = hotel;

            var size = hotelResponse.Content.Headers.ContentLength.GetValueOrDefault();
            return Response.Ok(statusCode: (int)hotelResponse.StatusCode, sizeBytes: (int)size,
                latencyMs: watch.ElapsedMilliseconds);
        });
        
        var scenario = ScenarioBuilder.CreateScenario("hotel", hotel);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("Hotel")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}