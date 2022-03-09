namespace TicketService.Api;

public class MinioConfiguration
{
    public MinioConfiguration(IConfiguration configuration)
    {
        Uri = configuration["Uri"];
        Username = configuration["Username"];
        Password = configuration["Password"];
    }

    public string Uri { get; }
    public string Username { get; }
    public string Password { get; }
}