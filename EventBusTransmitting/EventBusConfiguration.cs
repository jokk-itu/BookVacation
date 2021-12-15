namespace EventBusTransmitting;

public record EventBusConfiguration
{
    public string Host { get; init; }
    public int Port { get; init; }
    public string Username { get; init; }
    public string Password { get; init; }
}