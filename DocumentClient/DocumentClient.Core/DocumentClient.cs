using Microsoft.Extensions.Logging;
using Polly;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.Exceptions.Documents.Session;

namespace DocumentClient;

public class DocumentClient : IDocumentClient
{
    private readonly ILogger<DocumentClient> _logger;
    private readonly AsyncPolicy _policies;
    private readonly IAsyncDocumentSession _session;

    public DocumentClient(IAsyncDocumentSession session, ILogger<DocumentClient> logger)
    {
        _session = session;
        _logger = logger;
        _policies = Policy.WrapAsync(
            Policy.Handle<ConcurrencyException>()
                .Or<DatabaseLoadFailureException>()
                .WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(2 * retryCount),
                    (exception, time, retryCount, _) =>
                    {
                        _logger.LogDebug("{RavenException} occured, retried {RetryCount}, current wait {Elapsed}",
                            exception.GetType().Name,
                            retryCount, time.Milliseconds);
                    }),
            Policy.Handle<RequestedNodeUnavailableException>()
                .Or<AllTopologyNodesDownException>()
                .Or<DatabaseLoadTimeoutException>().Or<DatabaseDisabledException>()
                .Or<DatabaseConcurrentLoadTimeoutException>()
                .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(2), (exception, retryCount, time) =>
                {
                    _logger.LogDebug("{RavenException} occured, retried {RetryCount}, current wait {Elapsed}",
                        exception.GetType().Name,
                        retryCount, time.Milliseconds);
                }));
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _policies.ExecuteAsync(async () => await _session.Advanced.ExistsAsync(id, cancellationToken));
    }

    public async Task<T?> QueryAsync<T>(Func<IRavenQueryable<T>, Task<T>> query)
    {
        return await _policies.ExecuteAsync(async () => await query(_session.Query<T>()));
    }

    public async Task<T> LoadSingleAsync<T>(string id, CancellationToken cancellationToken = default)
    {
        return await _policies.ExecuteAsync(async () => await _session.LoadAsync<T>(id, cancellationToken));
    }

    public async Task<IDictionary<string, T>> LoadAsync<T>(IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        return await _policies.ExecuteAsync(async () => await _session.LoadAsync<T>(ids, cancellationToken));
    }

    public async Task<bool> StoreAsync<T>(T document, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _policies.ExecuteAsync(async () =>
            {
                await _session.StoreAsync(document, cancellationToken);
                await _session.SaveChangesAsync(cancellationToken);
                return true;
            });
        }
        catch (NonUniqueObjectException)
        {
            return false;
        }
    }

    public async Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        await _policies.ExecuteAsync(async () =>
        {
            await _session.SaveChangesAsync(cancellationToken);
        });
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _policies.ExecuteAsync(async () =>
            {
                if (!await _session.Advanced.ExistsAsync(id, cancellationToken))
                    return false;
                
                _session.Delete(id);
                await _session.SaveChangesAsync(cancellationToken);
                return true;
            });
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}