using System.Net.Http.Json;
using Core.Fakes;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;

namespace Core.Services;

public class FlightService
{
    private readonly HttpClient _httpClient;

    public FlightService(string flightUri)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(flightUri)
        };
    }

    public async Task<PostFlightResponse> PostFlightAsync(Guid airplaneId)
    {
        var flightRequestFaker = FlightFaker.GetFlightRequestFaker();
        var flightRequest = flightRequestFaker.Generate();
        flightRequest.AirplaneId = airplaneId;
        var response = await _httpClient.PostAsJsonAsync("api/v1/flight", flightRequest);
        response.EnsureSuccessStatusCode();
        var flightResponse = await response.Content.ReadFromJsonAsync<PostFlightResponse>();
        return flightResponse!;
    }

    public async Task<PostAirplaneResponse> PostAirplaneAsync()
    {
        var airplaneRequestFaker = AirplaneFaker.GetAirplaneRequestFaker();
        var airplaneRequest = airplaneRequestFaker.Generate();
        var response = await _httpClient.PostAsJsonAsync("/api/v1/airplane", airplaneRequest);
        response.EnsureSuccessStatusCode();
        var airplaneResponse = await response.Content.ReadFromJsonAsync<PostAirplaneResponse>();
        return airplaneResponse!;
    }
}