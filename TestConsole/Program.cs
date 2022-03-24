using CommandLine;
using TestConsole;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        if(options.Vacation)
            VacationLoad.Start();
        
        if(options.Airplane)
            AirplaneLoad.Start();
        
        if(options.Flight)
            FlightLoad.Start();
        
        if(options.Hotel)
            HotelLoad.Start();

        if (options.RentalCar)
            RentalCar.Start();
    })
    .WithNotParsed(errors =>
    {
        errors.Output();
    });