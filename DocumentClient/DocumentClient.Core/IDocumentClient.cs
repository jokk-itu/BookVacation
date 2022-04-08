using Raven.Client.Documents.Linq;

namespace DocumentClient;

public interface IDocumentClient
{
    Task<T?> QueryAsync<T>(Func<IRavenQueryable<T>, Task<T>> query);
    
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<T> LoadSingle<T>(string id, CancellationToken cancellationToken = default);
    Task<IDictionary<string, T>> LoadAsync<T>(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task StoreAsync<T>(T document, CancellationToken cancellationToken = default);
    Task UpdateAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}