using Microsoft.Extensions.DependencyInjection;

namespace Meta;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMetaContextAccessor(this IServiceCollection services)
    {
        return services.AddSingleton<IMetaContextAccessor, MetaContextAccessor>();
    }
}