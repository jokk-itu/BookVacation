using System.Diagnostics;
using System.Net.Http.Json;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using TestConsole.Constants;
using TestConsole.Fakes;

namespace TestConsole.Steps;

public static class PostFlightStep
{
    public static async Task<Response> PostFlight(IStepContext<HttpClient, Unit> context)
    {
        var flightRequestFaker = FlightFaker.GetFlightRequestFaker();
        var flightRequest = flightRequestFaker.Generate();
        flightRequest.AirPlaneId = (context.Data["airplane"] as PostAirplaneResponse)!.Id;
        var watch = Stopwatch.StartNew();
        var flightResponse = await context.Client.PostAsJsonAsync("http://localhost:5001/api/v1/flight", flightRequest);
        watch.Stop();
        flightResponse.EnsureSuccessStatusCode();
        var flight = await flightResponse.Content.ReadFromJsonAsync<PostFlightResponse>();
        context.Data[DataName.Flight] = flight;

        var size = flightResponse.Content.Headers.ContentLength.GetValueOrDefault();
        return Response.Ok(statusCode: (int)flightResponse.StatusCode, sizeBytes: (int)size,
            latencyMs: watch.ElapsedMilliseconds);
    }
}