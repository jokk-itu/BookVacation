using System.Diagnostics;
using System.Net.Http.Json;
using CarService.Contracts.RentalCar;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using HotelService.Contracts.CreateHotel;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;
using TestConsole.Constants;
using TestConsole.Steps;
using VacationService.Contracts.Vacation;

namespace TestConsole;

public static class VacationLoad
{
    public static void Start()
    {
        var httpFactory = ClientFactory.Create(
            "factory",
            clientCount: 10,
            initClient: (number, context) => Task.FromResult(new HttpClient()));

        var airplane = Step.Create(StepName.PostAirplane, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context => await PostAirplaneStep.PostAirplane(context));

        var flight = Step.Create(StepName.PostFlight, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context => await PostFlightStep.PostFlight(context));

        var hotel = Step.Create(StepName.PostHotel, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context => await PostHotelStep.PostHotel(context));

        var rentalCar = Step.Create(StepName.PostRentalCar, timeout: TimeSpan.FromSeconds(5),
            clientFactory: httpFactory, execute: async context => await PostRentalCarStep.PostRentalCar(context));

        var vacation = Step.Create("post_vacation", timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context =>
            {
                var airplaneResponse = context.Data[DataName.Airplane] as PostAirplaneResponse;
                var flightResponse = context.Data[DataName.Flight] as PostFlightResponse;
                var hotelResponse = context.Data[DataName.Hotel] as PostHotelResponse;
                var rentalCarResponse = context.Data[DataName.Car] as PostRentalCarResponse;

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
                    await context.Client.PostAsJsonAsync("http://localhost:5000/api/v1/vacation", vacationRequest);
                watch.Stop();
                vacationResponse.EnsureSuccessStatusCode();

                return Response.Ok(statusCode: (int)vacationResponse.StatusCode, latencyMs: watch.ElapsedMilliseconds);
            });

        var scenario = ScenarioBuilder.CreateScenario("vacation", airplane, flight, hotel, rentalCar, vacation)
            .WithLoadSimulations(Simulation.InjectPerSecRandom(10, 20, TimeSpan.FromMinutes(2)))
            .WithWarmUpDuration(TimeSpan.FromMinutes(1));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestSuite("Vacation")
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