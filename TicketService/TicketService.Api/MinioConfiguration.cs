namespace TicketService.Api;

public class MinioConfiguration
{
    public string Uri { get; }
    public string Username { get; }
    public string Password { get; }
    
    public MinioConfiguration(IConfiguration configuration)
    {
        Uri = configuration["Uri"];
        Username = configuration["Username"];
        Password = configuration["Password"];
    }
}