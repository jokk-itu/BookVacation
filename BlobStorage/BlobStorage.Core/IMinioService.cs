namespace BlobStorage;

public interface IMinioService
{
    Task<Stream?> ReadAsync(string bucket, string id, CancellationToken cancellationToken = default);

    Task<bool> CreateAsync(string bucket, string id, byte[] data,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string bucket, string id, CancellationToken cancellationToken = default);
}