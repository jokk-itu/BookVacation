using EventDispatcher;
using HotelService.Api;
using HotelService.Infrastructure;
using HotelService.Infrastructure.CourierActivities;
using MassTransit;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using Serilog.Events;

var logConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .AddEnvironmentVariables()
    .Build()
    .GetSection("Logging");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Seq(logConfiguration["SeqUri"])
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add serilog
    builder.Host.UseSerilog((context, serviceProvider, config) =>
    {
        var seqUri = context.Configuration["Logging:SeqUri"];
        config.WriteTo.Seq(seqUri)
            .Enrich.FromLogContext()
            .MinimumLevel.Override("HotelService", LogEventLevel.Information)
            .MinimumLevel.Override("EventDispatcher", LogEventLevel.Information)
            .MinimumLevel.Override("Raven", LogEventLevel.Information)
            .MinimumLevel.Warning();
    });

    // Add services to the container.
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddApiVersioning(config => { config.ReportApiVersions = true; });
    builder.Services.AddVersionedApiExplorer(config =>
    {
        config.GroupNameFormat = "'v'VVV";
        config.SubstituteApiVersionInUrl = true;
    });
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    builder.Services.AddEventBus(builder.Configuration,
        configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
    builder.Services.AddMassTransitHostedService();
    builder.Services.AddSystemMetrics();

    // Configure the HTTP request pipeline.
    var app = builder.Build();
    if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSerilogRequestLogging();
    app.UseAuthorization();

    app.MapMetrics();
    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Log.Error(e, "Unhandled exception during startup");
}
finally
{
    Log.CloseAndFlush();
}