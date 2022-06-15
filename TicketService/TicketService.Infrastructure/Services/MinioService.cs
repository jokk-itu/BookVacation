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

    public async Task<Stream?> GetTicketAsync(string bucket, string ticketId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), cancellationToken))
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), cancellationToken);

            await _minioClient.StatObjectAsync(new StatObjectArgs().WithBucket(bucket).WithObject(ticketId),
                cancellationToken: cancellationToken);
            var output = new MemoryStream();
            await _minioClient.GetObjectAsync(
                new GetObjectArgs().WithBucket(bucket).WithObject(ticketId)
                    .WithCallbackStream(stream => { stream.CopyTo(output); }),
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
            if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), cancellationToken))
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket),
                    cancellationToken: cancellationToken);

            var stream = new MemoryStream(data);
            await _minioClient.PutObjectAsync(
                new PutObjectArgs().WithBucket(bucket).WithObject(ticketId).WithObjectSize(data.Length)
                    .WithStreamData(stream), cancellationToken: cancellationToken);
            await stream.DisposeAsync();
            return true;
        }
        catch (MinioException)
        {
            return false;
        }
    }
}