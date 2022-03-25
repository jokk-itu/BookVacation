using Logging;
using Serilog;

var logConfiguration = new LoggingConfiguration(new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .AddEnvironmentVariables()
    .Build()
    .GetSection("Logging"));

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, serviceProvider, configuration) =>
{
    configuration.ConfigureLogging(logConfiguration);
});

builder.WebHost.ConfigureServices(services =>
{
    services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    services.AddCors(options =>
    {
        options.AddPolicy("Strict", policyBuilder =>
        {
            policyBuilder.AllowCredentials();
            policyBuilder.WithOrigins("localhost");
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowAnyHeader();
        });
    });
});

StartupLogger.Run(() =>
{
    var app = builder.Build();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors();
    app.MapReverseProxy();
    app.Run();
}, logConfiguration);