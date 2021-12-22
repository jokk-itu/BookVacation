using EventBusTransmitting;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;

namespace Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddApiVersioning(config => { config.ReportApiVersions = true; });
        services.AddVersionedApiExplorer(config =>
        {
            config.GroupNameFormat = "'v'VVV";
            config.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        services.AddEventBus(Configuration);
        services.AddMassTransitHostedService();
        services.AddSystemMetrics();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseSerilogRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var version in provider.ApiVersionDescriptions)
                options.SwaggerEndpoint(
                    $"/swagger/{version.GroupName}/swagger.json",
                    version.GroupName.ToUpper());
        });

        app.UseRouting();
        
        app.UseHttpMetrics();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics();
        });
    }
}