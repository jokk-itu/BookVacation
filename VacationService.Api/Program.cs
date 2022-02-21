using EventBusTransmitting;
using MassTransit;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using Serilog.Events;
using Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add serilog
builder.Host.UseSerilog((context, serviceProvider, config) =>
{
    var seqUri = context.Configuration["Logging:SeqUri"];
    config
        .WriteTo.Seq(seqUri)
        .WriteTo.Console(LogEventLevel.Debug)
        .Enrich.FromLogContext()
        .MinimumLevel.Override("VacationService", LogEventLevel.Information)
        .MinimumLevel.Override("EventBusTransmitting", LogEventLevel.Information)
        .MinimumLevel.Warning();
});

// Add services to the container.
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

builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddMassTransitHostedService();

builder.Services.AddSystemMetrics();

// Configure the HTTP request pipeline.
var app = builder.Build();
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseHttpMetrics();

app.UseAuthorization();

app.MapMetrics();
app.MapControllers();

app.Run();