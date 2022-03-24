using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Bogus;
using CarService.Contracts.RentalCar;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;

namespace TestConsole;

public static class RentalCar
{
    public static void Start()
    {
        var httpClient = new HttpClient();
        
        var rentalCarRequestFaker = new Faker<PostRentalCarRequest>().Rules((faker, request) =>
        {
            request.Color = "Blue";
            request.DayPrice = (short)Random.Shared.Next(1, 10000);
            request.CarCompanyName = faker.Company.CompanyName();
            request.RentingCompanyName = faker.Company.CompanyName();
            request.CarModelNumber = Guid.NewGuid();
        });
        
        var rentalCar = Step.Create("post_rentalcar", async context =>
        {
            var rentalCarRequest = rentalCarRequestFaker.Generate();
            var watch = Stopwatch.StartNew();
            var rentalCarResponse =
                await httpClient.PostAsJsonAsync("http://localhost:5003/api/v1/rentalcar", rentalCarRequest);
            watch.Stop();
            
            if(rentalCarResponse.StatusCode == HttpStatusCode.BadRequest)
                context.Logger.Information("BadRequest {Data}", System.Text.Json.JsonSerializer.Serialize(rentalCarRequest));
            
            rentalCarResponse.EnsureSuccessStatusCode();
            var rentalCar = await rentalCarResponse.Content.ReadFromJsonAsync<PostRentalCarResponse>();
            context.Data["rentalCar"] = rentalCar;

            var size = rentalCarResponse.Content.Headers.ContentLength.GetValueOrDefault();
            return Response.Ok(statusCode: (int)rentalCarResponse.StatusCode, sizeBytes: (int)size,
                latencyMs: watch.ElapsedMilliseconds);
        });
        
        var scenario = ScenarioBuilder.CreateScenario("rentalcar", rentalCar);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("RentalCar")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}