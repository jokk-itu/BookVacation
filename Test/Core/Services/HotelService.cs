using System.Net.Http.Json;
using HotelService.Contracts.CreateHotel;
using TestConsole.Fakes;

namespace TestConsole.Services;

public class HotelService
{
    private readonly HttpClient _httpClient;

    public HotelService(string hotelUri)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(hotelUri)
        };
    }

    public async Task<PostHotelResponse> PostHotel()
    {
        var hotelRequestFaker = HotelFaker.GetFlightRequestFaker();
        var hotelRequest = hotelRequestFaker.Generate();
        var response = await _httpClient.PostAsJsonAsync("api/v1/hotel", hotelRequest);
        response.EnsureSuccessStatusCode();
        var hotel = await response.Content.ReadFromJsonAsync<PostHotelResponse>();
        return hotel!;
    }
}