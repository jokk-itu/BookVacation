using System.Diagnostics;
using System.Net.Http.Json;
using FlightService.Contracts.Airplane;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using TestConsole.Constants;
using TestConsole.Fakes;

namespace TestConsole.Steps;

public static class PostAirplaneStep
{
    public static async Task<Response> PostAirplane(IStepContext<HttpClient, Unit> context)
    {
        var airplaneRequestFaker = AirplaneFaker.GetAirplaneRequestFaker();
        var airplaneRequest = airplaneRequestFaker.Generate();
        var watch = Stopwatch.StartNew();
        var airplaneResponse =
            await context.Client.PostAsJsonAsync("http://localhost:5001/api/v1/airplane", airplaneRequest);
        watch.Stop();
        airplaneResponse.EnsureSuccessStatusCode();
        var airplane = await airplaneResponse.Content.ReadFromJsonAsync<PostAirplaneResponse>();
        context.Data[DataName.Airplane] = airplane;
        var size = airplaneResponse.Content.Headers.ContentLength.GetValueOrDefault();
        return Response.Ok(statusCode: (int)airplaneResponse.StatusCode, sizeBytes: (int)size,
            latencyMs: watch.ElapsedMilliseconds);
    }
}