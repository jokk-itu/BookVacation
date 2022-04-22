using Raven.Client.Documents.Linq;

namespace DocumentClient;

public interface IDocumentClient
{
    Task<T?> QueryAsync<T>(Func<IRavenQueryable<T>, Task<T>> query);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<T> LoadSingleAsync<T>(string id, CancellationToken cancellationToken = default);
    Task<IDictionary<string, T>> LoadAsync<T>(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<bool> StoreAsync<T>(T document, CancellationToken cancellationToken = default);
    Task UpdateAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}