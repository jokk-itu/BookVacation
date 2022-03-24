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
            .MinimumLevel.Override("GatewayService", LogEventLevel.Information)
            .MinimumLevel.Warning();
    });

    //Add services
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Strict", policyBuilder =>
        {
            policyBuilder.AllowCredentials();
            policyBuilder.WithOrigins("localhost");
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowAnyHeader();
        });
    });

//Add pipeline setup
    var app = builder.Build();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors();
    app.MapReverseProxy();
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