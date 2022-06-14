using IntegrationConsole.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace IntegrationConsole.Commands;

public class VacationIntegrationCommand : Command<TestSettings>
{
    public override int Execute(CommandContext context, TestSettings settings)
    {
        var vacationService = new Core.Services.VacationService(settings.VacationUri, settings.FlightUri, settings.HotelUri, settings.CarUri);
        vacationService.PostVacationAsync().GetAwaiter().GetResult();
        AnsiConsole.MarkupLine("[green]Vacation test completed[/]");
        return 0;
    }
}