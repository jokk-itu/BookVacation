using Microsoft.Extensions.Logging;
using Minio;
using Minio.Exceptions;

namespace BlobStorage;

public class MinioService : IMinioService
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<MinioService> _logger;

    public MinioService(MinioClient minioClient, ILogger<MinioService> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }
    
    public async Task<Stream?> ReadAsync(string bucket, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _minioClient.StatObjectAsync(new StatObjectArgs().WithBucket(bucket).WithObject(id),
                cancellationToken: cancellationToken);
            var output = new MemoryStream();
            await _minioClient.GetObjectAsync(
                new GetObjectArgs().WithBucket(bucket).WithObject(id)
                    .WithCallbackStream(stream => { stream.CopyTo(output); }),
                cancellationToken: cancellationToken);
            return output;
        }
        catch (BucketNotFoundException)
        {
            _logger.LogWarning("Bucket {} does not exist", bucket);
            return null;
        }
        catch (ObjectNotFoundException)
        {
            _logger.LogDebug("Object with identifier {} does not exist", id);
            return null;
        }
        catch (MinioException e)
        {
            _logger.LogError(e, "Unknown error occured");
            return null;
        }
    }

    public async Task<bool> CreateAsync(string bucket, string id, byte[] data, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), cancellationToken))
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket),
                    cancellationToken: cancellationToken);

            var stream = new MemoryStream(data);
            await _minioClient.PutObjectAsync(
                new PutObjectArgs().WithBucket(bucket).WithObject(id).WithObjectSize(data.Length)
                    .WithStreamData(stream), cancellationToken: cancellationToken);
            await stream.DisposeAsync();
            return true;
        }
        catch (MinioException e)
        {
            _logger.LogError(e, "Unknown error occured");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string bucket, string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucket).WithObject(id),
                cancellationToken);
            return true;
        }
        catch (BucketNotFoundException)
        {
            _logger.LogWarning("Bucket {} does not exist", bucket);
            return false;
        }
        catch (ObjectNotFoundException)
        {
            _logger.LogWarning("Object {} does not exist", id);
            return false;
        }
        catch (MinioException e)
        {
            _logger.LogError(e, "Unknown error occured");
            return false;
        }
    }
}