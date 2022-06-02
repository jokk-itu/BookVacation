using NBomber.CSharp;
using Serilog;
using TestConsole.Constants;
using TestConsole.Steps;

namespace TestConsole;

public static class FlightLoad
{
    public static void Start()
    {
        var httpFactory = ClientFactory.Create(
            "factory",
            clientCount: 10,
            initClient: (number, context) => Task.FromResult(new HttpClient()));

        var airplane = Step.Create(StepName.PostAirplane, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context => await PostAirplaneStep.PostAirplane(context));

        var flight = Step.Create(StepName.PostFlight, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context => await PostFlightStep.PostFlight(context));

        var scenario = ScenarioBuilder.CreateScenario("flight", airplane, flight);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("Flight")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}