using System.Diagnostics;
using System.Net.Http.Json;
using Bogus;
using CarService.Contracts.RentalCar;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using HotelService.Contracts.CreateHotel;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;
using VacationService.Contracts.Vacation;

namespace TestConsole;

public static class VacationLoad
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
        var hotelRequestFaker = new Faker<PostHotelRequest>().Rules((faker, request) =>
        {
            request.Rooms = 2;
            request.Address = faker.Address.StreetAddress();
            request.City = faker.Address.City();
            request.Country = faker.Address.Country();
        });
        var rentalCarRequestFaker = new Faker<PostRentalCarRequest>().Rules((faker, request) =>
        {
            request.Color = "Blue";
            request.DayPrice = (short)Random.Shared.Next(1, 10000);
            request.CarCompanyName = faker.Company.CompanyName();
            request.RentingCompanyName = faker.Company.CompanyName();
            request.CarModelNumber = Guid.NewGuid();
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

        var rentalCar = Step.Create("post_rentalCar", async context =>
        {
            var rentalCarRequest = rentalCarRequestFaker.Generate();
            var watch = Stopwatch.StartNew();
            var rentalCarResponse =
                await httpClient.PostAsJsonAsync("http://localhost:5003/api/v1/rentalcar", rentalCarRequest);
            watch.Stop();
            rentalCarResponse.EnsureSuccessStatusCode();
            var rentalCar = await rentalCarResponse.Content.ReadFromJsonAsync<PostRentalCarResponse>();
            context.Data["rentalCar"] = rentalCar;

            var size = rentalCarResponse.Content.Headers.ContentLength.GetValueOrDefault();
            return Response.Ok(statusCode: (int)rentalCarResponse.StatusCode, sizeBytes: (int)size,
                latencyMs: watch.ElapsedMilliseconds);
        });

        var vacation = Step.Create("post_vacation", async context =>
        {
            var airplaneResponse = context.Data["airplane"] as PostAirplaneResponse;
            var flightResponse = context.Data["flight"] as PostFlightResponse;
            var hotelResponse = context.Data["hotel"] as PostHotelResponse;
            var rentalCarResponse = context.Data["rentalCar"] as PostRentalCarResponse;

            var vacationRequest = new PostVacationRequest
            {
                FlightId = flightResponse!.Id,
                FlightSeatId = airplaneResponse!.Seats.First().Id,
                HotelId = hotelResponse!.Id,
                HotelRoomId = hotelResponse.HotelRooms.First().Id,
                HotelFrom = DateTimeOffset.Now.AddDays(1),
                HotelTo = DateTimeOffset.Now.AddDays(2),
                RentalCarId = rentalCarResponse!.Id,
                RentingCompanyName = rentalCarResponse.RentingCompanyName,
                RentalCarFrom = DateTimeOffset.Now.AddDays(1),
                RentalCarTo = DateTimeOffset.Now.AddDays(2)
            };
            var watch = Stopwatch.StartNew();
            var vacationResponse =
                await httpClient.PostAsJsonAsync("http://localhost:5000/api/v1/vacation", vacationRequest);
            watch.Stop();
            vacationResponse.EnsureSuccessStatusCode();

            var size = vacationResponse.Content.Headers.ContentLength.GetValueOrDefault();
            return Response.Ok(statusCode: (int)vacationResponse.StatusCode, sizeBytes: (int)size,
                latencyMs: watch.ElapsedMilliseconds);
        });

        var scenario = ScenarioBuilder.CreateScenario("vacation", airplane, flight, hotel, rentalCar, vacation);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("Vacation")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}