using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace DocumentClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRavenDb(this IServiceCollection services, IConfiguration configuration)
    {
        var database = configuration["Database"];
        services.AddSingleton<IDocumentStore>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DocumentClient>>();
            var documentStore = new DocumentStore
            {
                Urls = configuration.GetSection("Urls").Get<string[]>(),
            };
            documentStore.Conventions.UseOptimisticConcurrency = true;

            documentStore.OnAfterSaveChanges += (sender, args) =>
            {
                using (logger.BeginScope(new Dictionary<string, object>
                       {
                           { "Database", args.Session.DatabaseName }, { "SessionId", args.Session.Id },
                           { "StoreId", args.Session.StoreIdentifier }
                       }))
                {
                    logger.LogInformation("Saved document {DocumentId}", args.DocumentId);
                }
            };
            documentStore.OnBeforeDelete += (sender, args) =>
            {
                using (logger.BeginScope(new Dictionary<string, object>
                       {
                           { "Database", args.Session.DatabaseName }, { "SessionId", args.Session.Id },
                           { "StoreId", args.Session.StoreIdentifier }
                       }))
                {
                    logger.LogInformation("Deleting document {DocumentId}", args.DocumentId);
                }
            };
            documentStore.OnBeforeQuery += (sender, args) =>
            {
                using (logger.BeginScope(new Dictionary<string, object>
                       {
                           { "Database", args.Session.DatabaseName }, { "SessionId", args.Session.Id },
                           { "StoreId", args.Session.StoreIdentifier }
                       }))
                {
                    logger.LogInformation("Querying document(s)");
                }
            };
            documentStore.OnBeforeStore += (sender, args) =>
            {
                using (logger.BeginScope(new Dictionary<string, object>
                       {
                           { "Database", args.Session.DatabaseName }, { "SessionId", args.Session.Id },
                           { "StoreId", args.Session.StoreIdentifier }
                       }))
                {
                    logger.LogInformation("Storing document {DocumentId}", args.DocumentId);
                }
            };

            documentStore.Initialize();
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
        services.AddTransient<IAsyncDocumentSession>(sp =>
            sp.GetRequiredService<IDocumentStore>().OpenAsyncSession(database));
        services.AddTransient<IDocumentClient, DocumentClient>();
        return services;
    }
}