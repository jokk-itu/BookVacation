using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace RavenClient.Core;

//Transient Service

//Handle exceptions for all functions in a try/catch

//Optionally return a result
public class RavenClient
{
    private readonly IAsyncDocumentSession _session;

    public RavenClient(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<T> QueryAsync<T>(Func<IRavenQueryable<T>, Task<T>> query)
    {
        return await query(_session.Query<T>());
    }

    public async Task StoreAsync<T>(T document)
    {
        await _session.StoreAsync(document);
        await _session.SaveChangesAsync();
    }
    
    public async Task UpdateAsync()
    {
        await _session.SaveChangesAsync();
    }

    public async Task DeleteAsync<T>(T key)
    {
        _session.Delete(key);
        await _session.SaveChangesAsync();
    }
}