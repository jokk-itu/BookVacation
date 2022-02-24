using HotelService.Domain.Entities.Nodes;

namespace HotelService.Domain.Entities.Edges;

public class RentHotelRentingRoom
{
    public uint Days { get; init; }

    public RentHotel FromNode { get; init; } = default!;

    public Room ToNode { get; init; } = default!;
}