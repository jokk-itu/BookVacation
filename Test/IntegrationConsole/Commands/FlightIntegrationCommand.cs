using Spectre.Console;
using Spectre.Console.Cli;
using TestConsole.Settings;
namespace TestConsole.Commands;

public class FlightIntegrationCommand : Command<TestSettings>
{
    public override int Execute(CommandContext context, TestSettings settings)
    {
        var flightService = new Services.FlightService(settings.FlightUri);
        var airplane = flightService.PostAirplaneAsync().GetAwaiter().GetResult();
        flightService.PostFlightAsync(airplane.Id).GetAwaiter().GetResult();
        AnsiConsole.MarkupLine("[green]Flight test completed[/]");
        return 0;
    }
}