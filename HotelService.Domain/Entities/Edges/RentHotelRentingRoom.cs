using HotelService.Api.Entities.Nodes;

namespace HotelService.Api.Entities.Edges;

public class RentHotelRentingRoom
{
    public uint Days { get; init; }

    public RentHotel FromNode { get; init; } = default!;

    public Room ToNode { get; init; } = default!;
}