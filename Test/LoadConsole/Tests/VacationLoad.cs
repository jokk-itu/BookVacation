using System.Diagnostics;
using NBomber.Contracts;
using NBomber.CSharp;
using Serilog;

namespace LoadConsole.Tests;

public class VacationLoad
{
    public static void Execute(Options options)
    {
        var httpFactory = ClientFactory.Create(
            "factory",
            clientCount: 10,
            initClient: (_, _) => Task.FromResult(new HttpClient()));

        var vacation = Step.Create("post_vacation", clientFactory: httpFactory,
            execute: async context =>
            {
                var watch = Stopwatch.StartNew();
                await new Core.Services.VacationService(options.VacationUri, options.FlightUri, options.HotelUri,
                    options.CarUri).PostVacationAsync();
                watch.Stop();
                return Response.Ok(statusCode: 202, sizeBytes: 0, latencyMs: watch.ElapsedMilliseconds);
            });

        var scenario = ScenarioBuilder.CreateScenario("vacation", vacation)
            .WithLoadSimulations(Simulation.RampPerSec(10, TimeSpan.FromSeconds(120)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestSuite("Vacation")
            .WithTestName("Vacation")
            .Run();
    }
}