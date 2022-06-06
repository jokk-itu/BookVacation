using System.Net.Http.Json;
using VacationService.Contracts.Vacation;

namespace TestConsole.Services;

public class VacationService
{
    private readonly string _flightUri;
    private readonly string _hotelUri;
    private readonly string _carUri;
    private readonly HttpClient _httpClient;
    
    public VacationService(string vacationUri, string flightUri, string hotelUri, string carUri)
    {
        _flightUri = flightUri;
        _hotelUri = hotelUri;
        _carUri = carUri;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(vacationUri)
        };
    }

    public async Task PostVacationAsync()
    {
        var flightService = new FlightService(_flightUri);
        var hotelService = new HotelService(_hotelUri);
        var carService = new Core.Services.CarService(_carUri);

        var airplane = await flightService.PostAirplaneAsync();
        var flight = await flightService.PostFlightAsync(airplane.Id);
        var hotel = await hotelService.PostHotel();
        var car = await carService.PostRentalCarAsync();

        var request = new PostVacationRequest
        {
            FlightId = flight.Id,
            FlightSeatId = Guid.NewGuid(),
            HotelId = hotel.Id,
            HotelFrom = DateTimeOffset.Now,
            HotelTo = DateTimeOffset.Now.AddDays(2),
            HotelRoomId = hotel.HotelRooms.First().Id,
            RentalCarId = car.Id,
            RentalCarFrom = DateTimeOffset.Now,
            RentalCarTo = DateTimeOffset.Now.AddDays(2),
            RentingCompanyName = car.RentingCompanyName
        };
        var response = await _httpClient.PostAsJsonAsync("/api/v1/vacation", request);
        response.EnsureSuccessStatusCode();
    }
}