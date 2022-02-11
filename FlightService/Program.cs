using EventBusTransmitting;
using FlightService.CourierActivities;
using FlightService.StateMachines.BookFlightStateMachine;
using FluentValidation.AspNetCore;
using MassTransit;
using MediatR;
using Neo4j.Driver;
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
    config.WriteTo.Seq(seqUri)
        .Enrich.FromLogContext()
        .MinimumLevel.Override("FlightService", LogEventLevel.Information)
        .MinimumLevel.Override("EventBusTransmitting", LogEventLevel.Information)
        .MinimumLevel.Override("Neo4j", LogEventLevel.Information)
        .MinimumLevel.Warning();
});

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddFluentValidation(options =>
{
    options.DisableDataAnnotationsValidation = false;
    options.AutomaticValidationEnabled = true;
    options.RegisterValidatorsFromAssemblyContaining<FlightService.Validators.AssemblyRegistration>();
});
builder.Services.AddMediatR(typeof(FlightService.Requests.AssemblyRegistration).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(config => { config.ReportApiVersions = true; });
builder.Services.AddVersionedApiExplorer(config =>
{
    config.GroupNameFormat = "'v'VVV";
    config.SubstituteApiVersionInUrl = true;
});
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddEventBus(builder.Configuration, configurator =>
{
    configurator.AddActivitiesFromNamespaceContaining<CourierActivitiesRegistration>();
    configurator.AddSagaStateMachine<BookFlightStateMachine, BookFlightStateMachineInstance>()
        .MongoDbRepository(mongodbConfigurator =>
        {
            mongodbConfigurator.Connection = builder.Configuration["Mongo:Uri"];
            mongodbConfigurator.DatabaseName = builder.Configuration["Mongo:Database"];
        });
});
builder.Services.AddSingleton(_ => GraphDatabase.Driver(
    builder.Configuration["Neo4j:Uri"],
    AuthTokens.Basic(
        builder.Configuration["Neo4j:Username"],
        builder.Configuration["Neo4j:Password"])));
builder.Services.AddMassTransitHostedService();
builder.Services.AddSystemMetrics();

// Configure the HTTP request pipeline.
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapMetrics();
app.MapControllers();

app.Run();