using System.Net.Http.Json;
using CarService.Contracts.RentalCar;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using HotelService.Contracts.CreateHotel;
using TestConsole.Fakes;
using VacationService.Contracts.Vacation;

namespace TestConsole;

public class VacationEndToEnd
{
    public static async Task Start()
    {
        var airplaneRequestFaker = AirplaneFaker.GetAirplaneRequestFaker();
        var flightRequestFaker = FlightFaker.GetFlightRequestFaker();
        var hotelRequestFaker = HotelFaker.GetFlightRequestFaker();
        var rentalCarRequestFaker = RentalCarFaker.GetRentalCarRequestFaker();

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
        var rentalCarResponse =
            await client.PostAsJsonAsync("http://localhost:5003/api/v1/rentalcar", rentalCarRequest);
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