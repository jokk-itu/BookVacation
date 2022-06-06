using System.Net.Http.Json;
using CarService.Contracts.RentalCar;
using TestConsole.Fakes;

namespace Core.Services;

public class CarService
{
    private readonly HttpClient _httpClient;

    public CarService(string carUri)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(carUri)
        };
    }

    public async Task<PostRentalCarResponse> PostRentalCarAsync()
    {
        var rentalCarRequestFaker = RentalCarFaker.GetRentalCarRequestFaker();
        var rentalCarRequest = rentalCarRequestFaker.Generate();
        var response = await _httpClient.PostAsJsonAsync("api/v1/rentalcar", rentalCarRequest);
        response.EnsureSuccessStatusCode();
        var rentalCar = await response.Content.ReadFromJsonAsync<PostRentalCarResponse>();
        return rentalCar!;
    }
}