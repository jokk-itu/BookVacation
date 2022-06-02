using NBomber.CSharp;
using Serilog;
using TestConsole.Constants;
using TestConsole.Steps;

namespace TestConsole;

public static class AirplaneLoad
{
    public static void Start()
    {
        var httpFactory = ClientFactory.Create(
            "factory",
            clientCount: 10,
            initClient: (number, context) => Task.FromResult(new HttpClient()));

        var airplane = Step.Create(StepName.PostAirplane, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context => await PostAirplaneStep.PostAirplane(context));
        var scenario = ScenarioBuilder.CreateScenario("airplane", airplane);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("Airplane")
            .WithTestSuite("Airplane")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}