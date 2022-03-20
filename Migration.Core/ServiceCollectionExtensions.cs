using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Migration.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRavenDb(this IServiceCollection services, IConfiguration configuration)
    {
        var database = configuration["Database"];
        services.AddSingleton<IDocumentStore>(_ =>
        {
            var documentStore = new DocumentStore
            {
                Urls = configuration.GetSection("Urls").Get<string[]>()
            }.Initialize();

            try
            {
                documentStore.Maintenance.Server.Send(
                    new CreateDatabaseOperation(new DatabaseRecord(database)));
            }
            catch (ConcurrencyException)
            {
                //Empty on purpose
            }

            return documentStore;
        });
        services.AddScoped<IAsyncDocumentSession>(sp =>
            sp.GetRequiredService<IDocumentStore>().OpenAsyncSession(database));
        return services;
    }
}