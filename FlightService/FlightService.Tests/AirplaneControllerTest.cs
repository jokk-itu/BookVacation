using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FlightService.Api;
using FlightService.Contracts.Airplane;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlightService.Tests;

public class AirplaneControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _api;

    public AirplaneControllerTest()
    {
        _api = new WebApplicationFactory<Program>();
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Post_ExpectCreated()
    {
        var client = _api.CreateClient();
        var request = new PostAirplaneRequest
        {
            ModelNumber = Guid.NewGuid(),
            AirplaneMakerName = "Boeing",
            AirlineName = "SAS",
            Seats = 20
        };
        var response = await client.PostAsJsonAsync("api/v1/airplane", request);
        response.EnsureSuccessStatusCode();
        var airplaneResponse = await response.Content.ReadFromJsonAsync<PostAirplaneResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(airplaneResponse);
    }
}