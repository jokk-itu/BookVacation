using Spectre.Console;
using Spectre.Console.Cli;
using TestConsole.Settings;

namespace TestConsole.Commands;

public class HotelIntegrationCommand : Command<TestSettings>
{
    public override int Execute(CommandContext context, TestSettings settings)
    {
        var hotelService = new Services.HotelService(settings.HotelUri);
        hotelService.PostHotel().GetAwaiter().GetResult();
        AnsiConsole.MarkupLine("[green]Hotel test completed[/]");
        return 0;
    }
}