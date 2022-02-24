using Serilog;
using Serilog.Events;

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
        policyBuilder.WithOrigins();
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