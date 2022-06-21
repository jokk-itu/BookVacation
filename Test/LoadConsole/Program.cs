using CommandLine;
using LoadConsole;
using LoadConsole.Tests;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        if (options.VacationLoad)
            VacationLoad.Execute(options);
    })
    .WithNotParsed(errors => { errors.Output(); });