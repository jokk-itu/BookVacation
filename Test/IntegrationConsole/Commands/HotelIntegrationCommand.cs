using IntegrationConsole.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace IntegrationConsole.Commands;

public class HotelIntegrationCommand : Command<TestSettings>
{
    public override int Execute(CommandContext context, TestSettings settings)
    {
        var hotelService = new Core.Services.HotelService(settings.HotelUri);
        hotelService.PostHotel().GetAwaiter().GetResult();
        AnsiConsole.MarkupLine("[green]Hotel test completed[/]");
        return 0;
    }
}