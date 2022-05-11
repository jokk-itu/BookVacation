using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using CarService.Contracts.RentalCar;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using TestConsole.Constants;
using TestConsole.Fakes;

namespace TestConsole.Steps;

public static class PostRentalCarStep
{
    public static async Task<Response> PostRentalCar(IStepContext<HttpClient, Unit> context)
    {
        var rentalCarRequestFaker = RentalCarFaker.GetRentalCarRequestFaker();
        var rentalCarRequest = rentalCarRequestFaker.Generate();
        var watch = Stopwatch.StartNew();
        var rentalCarResponse =
            await context.Client.PostAsJsonAsync("http://localhost:5003/api/v1/rentalcar", rentalCarRequest);
        watch.Stop();
            
        if(rentalCarResponse.StatusCode == HttpStatusCode.BadRequest)
            context.Logger.Information("BadRequest {Data}", System.Text.Json.JsonSerializer.Serialize(rentalCarRequest));
            
        rentalCarResponse.EnsureSuccessStatusCode();
        var rentalCar = await rentalCarResponse.Content.ReadFromJsonAsync<PostRentalCarResponse>();
        context.Data[DataName.Car] = rentalCar;

        var size = rentalCarResponse.Content.Headers.ContentLength.GetValueOrDefault();
        return Response.Ok(statusCode: (int)rentalCarResponse.StatusCode, sizeBytes: (int)size,
            latencyMs: watch.ElapsedMilliseconds);
    }
}