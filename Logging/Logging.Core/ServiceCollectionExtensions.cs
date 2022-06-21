using Logging.DelegatingHandlers;
using Logging.Enrichers;
using Meta;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        services.AddTransient<CorrelationIdDelegatingHandler>();
        services.AddTransient<RequestIdDelegatingHandler>();
        services.AddTransient<PerformanceDelegatingHandler>();

        services.AddTransient<CorrelationIdEnricher>();
        services.AddTransient<RequestIdEnricher>();

        services.ConfigureAll<HttpClientFactoryOptions>(options =>
        {
            options.HttpMessageHandlerBuilderActions.Add(builder =>
            {
                builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<CorrelationIdDelegatingHandler>());
                builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<RequestIdDelegatingHandler>());
                builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<PerformanceDelegatingHandler>());
            });
        });

        services.AddMetaContextAccessor();
        return services;
    }
}