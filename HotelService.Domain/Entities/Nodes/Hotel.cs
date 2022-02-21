namespace HotelService.Domain.Entities.Nodes;

public record Hotel
{
    public Guid Id { get; init; }

    public uint Stars { get; init; }
}