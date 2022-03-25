using CommandLine;
using TestConsole;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        if(options.VacationLoad)
            VacationLoad.Start();
        
        if(options.AirplaneLoad)
            AirplaneLoad.Start();
        
        if(options.FlightLoad)
            FlightLoad.Start();
        
        if(options.HotelLoad)
            HotelLoad.Start();

        if (options.RentalCarLoad)
            RentalCar.Start();
        
        if(options.VacationSingle)
            Vacation.Start().GetAwaiter().GetResult();
    })
    .WithNotParsed(errors =>
    {
        errors.Output();
    });