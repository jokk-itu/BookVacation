using CommandLine;

namespace LoadConsole;

public class Options
{
    [Option("vacation", Required = false, HelpText = "Whether or not the vacation load test run")]
    public bool VacationLoad { get; set; }

    [Option("loggeruri", Required = false)]
    public string LoggerUri => "http://localhost:5341";
    
    [Option("vacationuri", Required = false)]
    public string VacationUri => "http://localhost:5000";
    
    [Option("flighturi", Required = false)]
    public string FlightUri => "http://localhost:5001";
    
    [Option("hoteluri", Required = false)]
    public string HotelUri => "http://localhost:5002";
    
    [Option("caruri", Required = false)]
    public string CarUri => "http://localhost:5003";
    
    [Option("trackinguri", Required = false)]
    public string TrackingUri => "http://localhost:5004";
    
    [Option("ticketuri", Required = false)]
    public string TicketUri => "http://localhost:5005";
}