using HotelService.Entities.Nodes;

namespace HotelService.Entities.Edges;

public class RentRentingRoom
{
    public uint Days { get; init; }

    public Rent FromNode { get; init; } = default!;

    public Room ToNode { get; init; } = default!;
}