using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.AspNetCore.HealthChecks;
using Minio.Exceptions;
using Polly;

namespace BlobStorage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobStorage(this IServiceCollection services)
    {
        services.AddTransient(sp =>
            new BlobStorageConfiguration(sp.GetRequiredService<IConfiguration>().GetSection("BlobStorage")));
        services.AddTransient(sp => new RequestLogger(sp.GetRequiredService<ILogger<RequestLogger>>()));
        services.AddTransient<IMinioService, MinioService>();

        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<BlobStorageConfiguration>();
            var minioClient = new MinioClient()
                .WithEndpoint(configuration.Uri)
                .WithCredentials(configuration.Username, configuration.Password)
                .Build();
            minioClient.WithTimeout(5000);
            minioClient.SetTraceOn(serviceProvider.GetRequiredService<RequestLogger>());
            minioClient.WithRetryPolicy(async callback => await Policy
                .Handle<ConnectionException>()
                .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(retryCount * 2))
                .ExecuteAsync(async () => await callback()));
            return minioClient;
        });
        return services;
    }

    public static IServiceCollection AddBlobStorageHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks().AddMinio(serviceProvider => serviceProvider.GetRequiredService<MinioClient>(),
            tags: new[] { "live" });
        return services;
    }
}