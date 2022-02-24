using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FlightService.Contracts.DTO;
using FlightService.Contracts.GetFlight;
using FlightService.Contracts.PostFlight;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlightService.Tests;

public class FlightControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _api;

    public FlightControllerTests()
    {
        _api = new WebApplicationFactory<Program>();
    }
    
    [Trait("Category", "Integration")]
    [Fact]
    public async Task GetFlight_ExpectOk()
    {
        var client = _api.CreateClient();
        var postFlightRequest = new PostFlightRequest {From = DateTime.Now.AddDays(1), To = DateTime.Now.AddDays(2)};
        var postFlightResponse = await client.PostAsJsonAsync("api/v1/flight", postFlightRequest);
        postFlightResponse.EnsureSuccessStatusCode();
        var flight = await postFlightResponse.Content.ReadFromJsonAsync<Flight>();
        var getFlightResponse = await client.GetFromJsonAsync<GetFlightResponse>($"api/v1/flight/{flight!.Id}");
        
        Assert.Equal(postFlightRequest.From, getFlightResponse!.From);
        Assert.Equal(postFlightRequest.To, getFlightResponse!.To);
    }
    
    [Trait("Category", "Integration")]
    [Fact]
    public async Task GetFlight_ExpectNotFound()
    {
        var client = _api.CreateClient();
        var postFlightRequest = new PostFlightRequest {From = DateTime.Now, To = DateTime.Now.AddDays(2)};
        var postFlightResponse = await client.PostAsJsonAsync("api/v1/flight", postFlightRequest);
        postFlightResponse.EnsureSuccessStatusCode();
        var flight = await postFlightResponse.Content.ReadFromJsonAsync<Flight>();
        var getFlightResponse = await client.GetFromJsonAsync<GetFlightResponse>($"api/v1/flight/{flight!.Id}");
        
        Assert.Equal(postFlightRequest.From, getFlightResponse!.From);
        Assert.Equal(postFlightRequest.To, getFlightResponse!.To);
    }
}