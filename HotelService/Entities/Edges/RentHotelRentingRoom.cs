using HotelService.Entities.Nodes;

namespace HotelService.Entities.Edges;

public class RentHotelRentingRoom
{
    public uint Days { get; init; }

    public RentHotel FromNode { get; init; } = default!;

    public Room ToNode { get; init; } = default!;
}