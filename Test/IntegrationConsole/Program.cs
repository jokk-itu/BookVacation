using IntegrationConsole.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.CaseSensitivity(CaseSensitivity.None);
    config.SetApplicationName("test");
    config.ValidateExamples();
    config.AddCommand<FlightIntegrationCommand>("flight");
    config.AddCommand<HotelIntegrationCommand>("hotel");
    config.AddCommand<CarIntegrationCommand>("car");
    config.AddCommand<VacationIntegrationCommand>("vacation");
});
return app.Run(args);