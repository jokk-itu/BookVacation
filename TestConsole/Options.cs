using CommandLine;

namespace TestConsole;

public class Options
{
    [Option('v', "vacation", Required = false, HelpText = "Whether or not the vacation load test run")]
    public bool Vacation { get; set; }
    
    [Option('a', "airplane", Required = false, HelpText = "Whether or not the airplane load test run")]
    public bool Airplane { get; set; }
    
    [Option('f', "flight", Required = false, HelpText = "Whether or not the flight load test run")]
    public bool Flight { get; set; }
    
    [Option('h', "hotel", Required = false, HelpText = "Whether or not the hotel load test run")]
    public bool Hotel { get; set; }
    
    [Option('r', "rentalcar", Required = false, HelpText = "Whether or not the rentalcar load test run")]
    public bool RentalCar { get; set; }
}