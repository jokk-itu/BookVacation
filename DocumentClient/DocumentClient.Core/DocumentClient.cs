using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;

namespace DocumentClient;

public class DocumentClient : IDocumentClient
{
    private readonly ILogger<DocumentClient> _logger;
    private readonly AsyncPolicyWrap _policies;
    private readonly IAsyncDocumentSession _session;

    public DocumentClient(IAsyncDocumentSession session, ILogger<DocumentClient> logger)
    {
        _session = session;
        _logger = logger;
        _policies = Policy.WrapAsync(
            Policy.Handle<ConcurrencyException>().WaitAndRetryAsync(5, retryCount =>
            {
                _logger.LogDebug("{RavenException} retried {RetryCount}", nameof(ConcurrencyException), retryCount);
                return TimeSpan.FromSeconds(2 * retryCount);
            }),
            Policy.Handle<RequestedNodeUnavailableException>().WaitAndRetryForeverAsync(retryCount =>
            {
                _logger.LogDebug("{RavenException} retried {RetryCount}", nameof(RequestedNodeUnavailableException),
                    retryCount);
                return TimeSpan.FromSeconds(1);
            }),
            Policy.Handle<AllTopologyNodesDownException>().WaitAndRetryForeverAsync(retryCount =>
            {
                _logger.LogDebug("{RavenException} retried {RetryCount}", nameof(AllTopologyNodesDownException),
                    retryCount);
                return TimeSpan.FromSeconds(2 * retryCount);
            }),
            Policy.Handle<DatabaseLoadTimeoutException>().WaitAndRetryForeverAsync(retryCount =>
            {
                _logger.LogDebug("{RavenException} retried {RetryCount}", nameof(DatabaseLoadTimeoutException),
                    retryCount);
                return TimeSpan.FromSeconds(2);
            }),
            Policy.Handle<DatabaseDisabledException>().WaitAndRetryForeverAsync(retryCount =>
            {
                _logger.LogDebug("{RavenException} retried {RetryCount}", nameof(DatabaseDisabledException),
                    retryCount);
                return TimeSpan.FromSeconds(2);
            }),
            Policy.Handle<DatabaseConcurrentLoadTimeoutException>().WaitAndRetryForeverAsync(retryCount =>
            {
                _logger.LogDebug("{RavenException} retried {RetryCount}",
                    nameof(DatabaseConcurrentLoadTimeoutException), retryCount);
                return TimeSpan.FromSeconds(2);
            }),
            Policy.Handle<DatabaseLoadFailureException>().WaitAndRetryAsync(5, retryCount =>
            {
                _logger.LogDebug("{RavenException} retried {RetryCount}", nameof(DatabaseLoadFailureException),
                    retryCount);
                return TimeSpan.FromSeconds(2 * retryCount);
            }));
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await Execute(async () => await _session.Advanced.ExistsAsync(id, cancellationToken));
    }

    public async Task<T?> QueryAsync<T>(Func<IRavenQueryable<T>, Task<T>> query)
    {
        return await Execute(async () => await query(_session.Query<T>()));
    }

    public async Task<T> LoadSingle<T>(string id, CancellationToken cancellationToken = default)
    {
        return await Execute(async () => await _session.LoadAsync<T>(id, cancellationToken));
    }

    public async Task<IDictionary<string, T>> LoadAsync<T>(IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        return await Execute(async () => await _session.LoadAsync<T>(ids, cancellationToken));
    }

    public async Task StoreAsync<T>(T document, CancellationToken cancellationToken = default)
    {
        await Execute(async () =>
        {
            await _session.StoreAsync(document, cancellationToken);
            await _session.SaveChangesAsync(cancellationToken);
        });
    }

    public async Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        await Execute(async () => await _session.SaveChangesAsync(cancellationToken));
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        //System.InvalidOperationException is thrown when key doesn't exist
        await Execute(async () =>
        {
            _session.Delete(id);
            await _session.SaveChangesAsync(cancellationToken);
        });
    }

    private async Task<T> Execute<T>(Func<Task<T>> func)
    {
        return await _policies.ExecuteAsync(func);
    }

    private async Task Execute(Func<Task> func)
    {
        await _policies.ExecuteAsync(func);
    }
}