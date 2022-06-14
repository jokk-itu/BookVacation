using IntegrationConsole.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace IntegrationConsole.Commands;

public class CarIntegrationCommand : Command<TestSettings>
{
    public override int Execute(CommandContext context, TestSettings settings)
    {
        var carService = new Core.Services.CarService(settings.CarUri);
        carService.PostRentalCarAsync().GetAwaiter().GetResult();
        AnsiConsole.MarkupLine("[green]Car test completed[/]");
        return 0;
    }
}