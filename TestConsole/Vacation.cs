using System.Net.Http.Json;
using Bogus;
using CarService.Contracts.RentalCar;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using HotelService.Contracts.CreateHotel;
using VacationService.Contracts.Vacation;

namespace TestConsole;

public class Vacation
{
    public static async Task Start()
    {
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

        var client = new HttpClient();
        
        var airplaneRequest = airplaneRequestFaker.Generate();
        var airplaneResponse = await client.PostAsJsonAsync("http://localhost:5001/api/v1/airplane", airplaneRequest);
        airplaneResponse.EnsureSuccessStatusCode();
        var airplane = await airplaneResponse.Content.ReadFromJsonAsync<PostAirplaneResponse>();
        
        var flightRequest = flightRequestFaker.Generate();
        flightRequest.AirPlaneId = airplane.Id;
        var flightResponse = await client.PostAsJsonAsync("http://localhost:5001/api/v1/flight", flightRequest);
        flightResponse.EnsureSuccessStatusCode();
        var flight = await flightResponse.Content.ReadFromJsonAsync<PostFlightResponse>();
        
        var hotelRequest = hotelRequestFaker.Generate();
        var hotelResponse = await client.PostAsJsonAsync("http://localhost:5002/api/v1/hotel", hotelRequest);
        hotelResponse.EnsureSuccessStatusCode();
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<PostHotelResponse>();
        
        var rentalCarRequest = rentalCarRequestFaker.Generate();
        var rentalCarResponse = await client.PostAsJsonAsync("http://localhost:5003/api/v1/rentalcar", rentalCarRequest);
        rentalCarResponse.EnsureSuccessStatusCode();
        var rentalCar = await rentalCarResponse.Content.ReadFromJsonAsync<PostRentalCarResponse>();

        var vacationRequest = new PostVacationRequest
        {
            FlightId = flight.Id,
            FlightSeatId = airplane.Seats.First().Id,
            HotelId = hotel.Id,
            HotelRoomId = hotel.HotelRooms.First().Id,
            HotelFrom = DateTimeOffset.Now.AddDays(1),
            HotelTo = DateTimeOffset.Now.AddDays(2),
            RentalCarId = rentalCar.Id,
            RentingCompanyName = rentalCar.RentingCompanyName,
            RentalCarFrom = DateTimeOffset.Now.AddDays(1),
            RentalCarTo = DateTimeOffset.Now.AddDays(2)
        };
        var vacationResponse = await client.PostAsJsonAsync("http://localhost:5000/api/v1/vacation", vacationRequest);
        vacationResponse.EnsureSuccessStatusCode();
    }
}