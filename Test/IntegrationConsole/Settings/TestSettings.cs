using Spectre.Console.Cli;

namespace TestConsole.Settings;

public class TestSettings : CommandSettings
{
    [CommandOption("-l|--log")] public string LoggerUri => "http://localhost:5341";
    
    [CommandOption("-v|--vacation")] public string VacationUri => "http://localhost:5000";

    [CommandOption("-f|--flight")] public string FlightUri => "http://localhost:5001";
    
    [CommandOption("-h|--hotel")] public string HotelUri => "http://localhost:5002";
    
    [CommandOption("-c|--car")] public string CarUri => "http://localhost:5003";
    
    [CommandOption("-r|--tracking")] public string TrackingUri => "http://localhost:5004";
    
    [CommandOption("-i|--ticket")] public string TicketUri => "http://localhost:5005";

}