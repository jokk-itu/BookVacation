using NBomber.CSharp;
using Serilog;
using TestConsole.Constants;
using TestConsole.Steps;

namespace TestConsole;

public static class HotelLoad
{
    public static void Start()
    {
        var httpFactory = ClientFactory.Create(
            "factory",
            clientCount: 10,
            initClient: (number, context) => Task.FromResult(new HttpClient()));

        var hotel = Step.Create(StepName.PostHotel, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory,
            execute: async context => await PostHotelStep.PostHotel(context));
        var scenario = ScenarioBuilder.CreateScenario("hotel", hotel);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("Hotel")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}