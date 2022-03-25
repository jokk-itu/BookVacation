using CommandLine;

namespace TestConsole;

public class Options
{
    [Option('a', "vacationload", Required = false, HelpText = "Whether or not the vacation load test run")]
    public bool VacationLoad { get; set; }
    
    [Option('b', "airplaneload", Required = false, HelpText = "Whether or not the airplane load test run")]
    public bool AirplaneLoad { get; set; }
    
    [Option('c', "flightload", Required = false, HelpText = "Whether or not the flight load test run")]
    public bool FlightLoad { get; set; }
    
    [Option('d', "hotelload", Required = false, HelpText = "Whether or not the hotel load test run")]
    public bool HotelLoad { get; set; }
    
    [Option('e', "rentalcarload", Required = false, HelpText = "Whether or not the rentalcar load test run")]
    public bool RentalCarLoad { get; set; }
    
    [Option('f', "vacationsingle", Required = false, HelpText = "Whether or not the vacation single test run")]
    public bool VacationSingle { get; set; }
}