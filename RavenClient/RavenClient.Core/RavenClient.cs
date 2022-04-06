using Polly;
using Polly.Wrap;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;

namespace RavenClient.Core;

public class RavenClient : IRavenClient
{
    private readonly IAsyncDocumentSession _session;
    private readonly AsyncPolicyWrap _policies;

    public RavenClient(IAsyncDocumentSession session)
    {
        _session = session;
        _policies = Policy.WrapAsync(
            Policy.Handle<ConcurrencyException>().WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(2 * retryCount)),
            Policy.Handle<RequestedNodeUnavailableException>().WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1)),
            Policy.Handle<AllTopologyNodesDownException>().WaitAndRetryForeverAsync(retryCount => TimeSpan.FromSeconds(retryCount * 2)),
            Policy.Handle<DatabaseLoadTimeoutException>().WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(2)),
            Policy.Handle<DatabaseDisabledException>().WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(2)),
            Policy.Handle<DatabaseConcurrentLoadTimeoutException>().WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(2)),
            Policy.Handle<DatabaseLoadFailureException>().WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(2 * retryCount)));
    }

    public async Task<T> QueryAsync<T>(Func<IRavenQueryable<T>, Task<T>> query)
    {
        return await query(_session.Query<T>());
    }

    public async Task StoreAsync<T>(T document)
    {
        await Execute(async () =>
        {
            await _session.StoreAsync(document);
            await _session.SaveChangesAsync(); 
        });
    }
    
    public async Task UpdateAsync()
    {
        await _session.SaveChangesAsync();
    }

    public async Task DeleteAsync<T>(T key)
    {
        await Execute(async () =>
        {
            _session.Delete(key);
            await _session.SaveChangesAsync();
        });
    }

    private async Task Execute(Func<Task> func)
    {
        await _policies.ExecuteAsync(func);
    }
}