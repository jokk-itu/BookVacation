using CommandLine;
using TestConsole;
using TestConsole.Tests;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        if (options.VacationLoad)
            VacationLoad.Start();

        if (options.AirplaneLoad)
            AirplaneLoad.Start();

        if (options.FlightLoad)
            FlightLoad.Start();

        if (options.HotelLoad)
            HotelLoad.Start();

        if (options.RentalCarLoad)
            RentalCarLoad.Start();

        if (options.VacationSingle)
            VacationEndToEnd.Start().GetAwaiter().GetResult();
    })
    .WithNotParsed(errors => { errors.Output(); });