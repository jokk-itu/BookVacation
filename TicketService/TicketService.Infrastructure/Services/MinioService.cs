using Minio;
using Minio.Exceptions;

namespace TicketService.Infrastructure.Services;

public class MinioService : IMinioService
{
    private readonly MinioClient _minioClient;

    public MinioService(MinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<Stream?> FetchTicketAsync(string bucket, string ticketId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _minioClient.BucketExistsAsync(bucket, cancellationToken))
                await _minioClient.MakeBucketAsync(bucket, cancellationToken: cancellationToken);

            await _minioClient.StatObjectAsync(bucket, ticketId, cancellationToken: cancellationToken);
            var output = new MemoryStream();
            await _minioClient.GetObjectAsync(bucket, ticketId, stream => { stream.CopyTo(output); },
                cancellationToken: cancellationToken);
            return output;
        }
        catch (MinioException)
        {
            return null;
        }
    }

    public async Task<bool> PutTicketAsync(string bucket, string ticketId, byte[] data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _minioClient.BucketExistsAsync(bucket, cancellationToken))
                await _minioClient.MakeBucketAsync(bucket, cancellationToken: cancellationToken);
            
            var stream = new MemoryStream(data);
            await _minioClient.PutObjectAsync(bucket, ticketId, stream, data.Length, cancellationToken: cancellationToken);
            await stream.DisposeAsync();
            return true;
        }
        catch (MinioException)
        {
            return false;
        }
    }
}