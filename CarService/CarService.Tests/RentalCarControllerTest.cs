using System.Net;
using System.Net.Http.Json;
using CarService.Api;
using CarService.Contracts.RentalCar;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CarService.Tests;

public class RentalCarControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _api;

    public RentalCarControllerTest()
    {
        _api = new WebApplicationFactory<Program>();
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Post_ExpectCreated()
    {
        var client = _api.CreateClient();
        var request = new PostRentalCarRequest
        {
            CarModelNumber = Guid.NewGuid(),
            CarCompanyName = "Mercedes",
            RentingCompanyName = "EuropeCar",
            DayPrice = 12,
            Color = "Blue"
        };
        var response = await client.PostAsJsonAsync("api/v1/rentalcar", request);
        var rentalCarResponse = await response.Content.ReadFromJsonAsync<PostRentalCarResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(rentalCarResponse);
    }
}