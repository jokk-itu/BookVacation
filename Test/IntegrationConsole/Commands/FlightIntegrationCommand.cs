using IntegrationConsole.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace IntegrationConsole.Commands;

public class FlightIntegrationCommand : Command<TestSettings>
{
    public override int Execute(CommandContext context, TestSettings settings)
    {
        var flightService = new Core.Services.FlightService(settings.FlightUri);
        var airplane = flightService.PostAirplaneAsync().GetAwaiter().GetResult();
        flightService.PostFlightAsync(airplane.Id).GetAwaiter().GetResult();
        AnsiConsole.MarkupLine("[green]Flight test completed[/]");
        return 0;
    }
}