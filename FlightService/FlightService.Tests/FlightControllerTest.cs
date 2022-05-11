using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FlightService.Api;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlightService.Tests;

public class FlightControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _api;

    public FlightControllerTest()
    {
        _api = new WebApplicationFactory<Program>();
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Post_ExpectCreated()
    {
        var client = _api.CreateClient();
        var airplaneRequest = new PostAirplaneRequest
        {
            ModelNumber = Guid.NewGuid(),
            AirplaneMakerName = "Boeing",
            AirlineName = "SAS",
            Seats = 20
        };
        var airplaneResponse = await client.PostAsJsonAsync("api/v1/airplane", airplaneRequest);
        airplaneResponse.EnsureSuccessStatusCode();
        var airplane = await airplaneResponse.Content.ReadFromJsonAsync<PostAirplaneResponse>();

        var flightRequest = new PostFlightRequest
        {
            From = DateTimeOffset.Now.AddDays(1),
            To = DateTimeOffset.Now.AddDays(2),
            FromAirport = "Kastrup",
            ToAirport = "Karup",
            AirplaneId = airplane!.Id,
            Price = 1200
        };
        var flightResponse = await client.PostAsJsonAsync("api/v1/flight", flightRequest);
        flightResponse.EnsureSuccessStatusCode();
        var flight = await flightResponse.Content.ReadFromJsonAsync<PostFlightResponse>();

        Assert.Equal(HttpStatusCode.Created, flightResponse.StatusCode);
        Assert.NotNull(flight);
    }
}