using NBomber.CSharp;
using Serilog;
using TestConsole.Constants;
using TestConsole.Steps;

namespace TestConsole;

public static class RentalCar
{
    public static void Start()
    {
        var httpFactory = ClientFactory.Create(
            name: "factory",
            clientCount: 10,
            initClient: (number, context) => Task.FromResult(new HttpClient()));
        
        var rentalCar = Step.Create(StepName.PostRentalCar, timeout: TimeSpan.FromSeconds(5), clientFactory: httpFactory, execute: async context => await PostRentalCarStep.PostRentalCar(context));
        var scenario = ScenarioBuilder.CreateScenario("rentalcar", rentalCar);

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestName("RentalCar")
            .WithLoggerConfig(() =>
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .WriteTo.Seq("http://localhost:5341")
            )
            .Run();
    }
}