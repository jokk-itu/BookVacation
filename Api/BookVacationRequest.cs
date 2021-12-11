using System;

namespace Api
{
    public record VacationRequest
    {
        #region Flight
        
        public Guid FlightId { get; init; }
        public int SeatId { get; init; }
        
        #endregion
        
        #region Hotel
        public Guid HotelId { get; init; }
        public uint RentHotelDays { get; init; }
        public Guid RoomId { get; init; }
        
        #endregion

        #region RentCar
        
        public Guid RentCarId { get; init; }
        public decimal CarPrice { get; init; }
        
        #endregion
    }
}