using Raven.Client.Documents.Linq;

namespace RavenClient.Core;

public interface IRavenClient
{
    Task<T> QueryAsync<T>(Func<IRavenQueryable<T>, Task<T>> query);
    Task StoreAsync<T>(T document);
    Task UpdateAsync();
    Task DeleteAsync<T>(T key);
}