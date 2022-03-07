using EventDispatcher;
using MassTransit;
using MediatR;
using Minio;
using Neo4j.Driver;
using Prometheus.SystemMetrics;
using Serilog;
using Serilog.Events;
using TicketService.Api;
using TicketService.Infrastructure.CourierActivities;
using TicketService.Infrastructure.Requests;
using TicketService.Infrastructure.Services;

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
            .MinimumLevel.Override("TicketService", LogEventLevel.Information)
            .MinimumLevel.Override("EventDispatcher", LogEventLevel.Information)
            .MinimumLevel.Override("Neo4j", LogEventLevel.Information)
            .MinimumLevel.Warning();
    });

    // Add services to the container.
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.AddMediatR(typeof(MediatorRegistration).Assembly);
    builder.Services.AddTransient(sp =>
        new MinioConfiguration(sp.GetRequiredService<IConfiguration>().GetSection("Minio")));
    builder.Services.AddTransient(sp => new MinioLogger(sp.GetRequiredService<ILogger<MinioLogger>>()));
    builder.Services.AddTransient<IMinioService, MinioService>();
    builder.Services.AddSingleton(sp =>
    {
        var configuration = sp.GetRequiredService<MinioConfiguration>();
        var minioClient = new MinioClient(configuration.Uri, configuration.Username, configuration.Password);
        minioClient.WithTimeout(5000);
        minioClient.SetTraceOn(sp.GetRequiredService<MinioLogger>());
        minioClient.WithRetryPolicy(async callback =>
        {
            const int maxRetry = 5;

            Exception? exception = null;
            for (var i = 0; i < maxRetry; ++i)
            {
                try
                {
                    return await callback();
                }
                catch (ConnectionException? e)
                {
                    exception = e;
                    await Task.Delay(TimeSpan.FromSeconds(i));
                }
            }

            throw exception!;
        });
        return minioClient;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddApiVersioning(config => { config.ReportApiVersions = true; });
    builder.Services.AddVersionedApiExplorer(config =>
    {
        config.GroupNameFormat = "'v'VVV";
        config.SubstituteApiVersionInUrl = true;
    });
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    builder.Services.AddMediatR(typeof(MediatorRegistration).Assembly);
    builder.Services.AddEventBus(builder.Configuration,
        configurator => { configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>(); });
    builder.Services.AddSingleton(_ => GraphDatabase.Driver(
        builder.Configuration["Neo4j:Uri"],
        AuthTokens.Basic(
            builder.Configuration["Neo4j:Username"],
            builder.Configuration["Neo4j:Password"])));

    builder.Services.AddMassTransitHostedService();
    builder.Services.AddSystemMetrics();

    var app = builder.Build();
    if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();

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