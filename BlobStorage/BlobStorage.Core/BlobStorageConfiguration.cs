using Microsoft.Extensions.Configuration;

namespace BlobStorage;

public class BlobStorageConfiguration : IBlobStorageConfiguration
{
    public BlobStorageConfiguration(IConfiguration configuration)
    {
        Uri = configuration["Uri"];
        Username = configuration["Username"];
        Password = configuration["Password"];
    }

    public string Uri { get; }
    public string Username { get; }
    public string Password { get; }
}