using System;

namespace Contracts.Vacation;

public record VacationRequest
{
    #region Flight

    public Guid FlightId { get; init; }
    public int SeatId { get; init; }

    #endregion

    #region Hotel

    public Guid HotelId { get; init; }
    public int RentHotelDays { get; init; }
    public Guid RoomId { get; init; }

    #endregion

    #region RentCar

    public Guid CarId { get; init; }
    public Guid RentingCompanyId { get; init; }
    public uint RentCarDays { get; init; }

    #endregion
}