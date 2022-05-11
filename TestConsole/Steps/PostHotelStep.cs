using System.Diagnostics;
using System.Net.Http.Json;
using HotelService.Contracts.CreateHotel;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using TestConsole.Constants;
using TestConsole.Fakes;

namespace TestConsole.Steps;

public static class PostHotelStep
{
    public static async Task<Response> PostHotel(IStepContext<HttpClient, Unit> context)
    {
        var hotelRequestFaker = HotelFaker.GetFlightRequestFaker();
        var hotelRequest = hotelRequestFaker.Generate();
        var watch = Stopwatch.StartNew();
        var hotelResponse = await context.Client.PostAsJsonAsync("http://localhost:5002/api/v1/hotel", hotelRequest);
        watch.Stop();
        hotelResponse.EnsureSuccessStatusCode();
        var hotel = await hotelResponse.Content.ReadFromJsonAsync<PostHotelResponse>();
        context.Data[DataName.Hotel] = hotel;

        var size = hotelResponse.Content.Headers.ContentLength.GetValueOrDefault();
        return Response.Ok(statusCode: (int)hotelResponse.StatusCode, sizeBytes: (int)size,
            latencyMs: watch.ElapsedMilliseconds);
    }
}