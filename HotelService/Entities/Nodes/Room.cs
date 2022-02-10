namespace HotelService.Entities.Nodes;

public record Room
{
    public Guid Id { get; init; }

    public uint People { get; init; }

    public int Floor { get; init; }
    
    public uint DayPrice { get; init; }
}