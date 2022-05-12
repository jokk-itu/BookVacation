using CarService.Contracts.RentalDeal;
using FlightService.Contracts.FlightReservation;
using HotelService.Contracts.HotelRoomReservationActivity;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.AspNetCore.Mvc;
using TicketService.Contracts.CreateVacationTickets;
using VacationService.Contracts.Vacation;

namespace VacationService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[ControllerName("vacation")]
[Route("api/v{version:apiVersion}/[controller]")]
public class VacationController : ControllerBase
{
    private readonly IBusControl _bus;

    public VacationController(IBusControl bus)
    {
        _bus = bus;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] PostVacationRequest request)
    {
        var builder = new RoutingSlipBuilder(NewId.NextGuid());

        builder.AddActivity("BookFlight",
            new Uri("exchange:flight-reservation_execute"),
            new FlightReservationArgument
            {
                FlightId = request.FlightId, 
                SeatId = request.FlightSeatId
            });

        builder.AddActivity("BookHotel",
            new Uri("exchange:hotel-room-reservation_execute"),
            new HotelRoomReservationArgument
            {
                HotelId = request.HotelId,
                From = request.HotelFrom,
                To = request.HotelTo,
                RoomId = request.HotelRoomId
            });

        builder.AddActivity("RentCar",
            new Uri("exchange:rental-deal_execute"),
            new RentalDealArgument
            {
                RentalCarId = request.RentalCarId,
                RentFrom = request.RentalCarFrom,
                RentTo = request.RentalCarTo
            });

        builder.AddActivity("CreateVacationTicket",
            new Uri("exchange:create-vacation-ticket_execute"),
            new CreateVacationTicketArgument
            {
                FlightId = request.FlightId,
                HotelId = request.HotelId,
                RoomId = request.HotelRoomId,
                CarId = request.RentalCarId,
                RentingCompanyName = request.RentingCompanyName
            });

        builder.AddSubscription(
            new Uri("exchange:routing-slip-event"),
            RoutingSlipEvents.All, RoutingSlipEventContents.None);

        var routingSlip = builder.Build();

        await _bus.Execute(routingSlip);

        return Accepted();
    }
}