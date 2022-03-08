using CarService.Contracts.RentCarActivity;
using FlightService.Contracts.BookFlightActivity;
using HotelService.Contracts.BookHotelActivity;
using MassTransit;
using MassTransit.Courier;
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
    public async Task<IActionResult> PostAsync([FromBody] VacationRequest request)
    {
        var builder = new RoutingSlipBuilder(NewId.NextGuid());

        builder.AddActivity("BookFlight",
            new Uri("queue:book-flight_execute"),
            new BookFlightArgument { FlightId = request.FlightId, SeatId = request.SeatId });

        builder.AddActivity("BookHotel",
            new Uri("queue:book-hotel_execute"),
            new BookHotelArgument { HotelId = request.HotelId, Days = request.RentHotelDays, RoomId = request.RoomId });

        builder.AddActivity("RentCar",
            new Uri("queue:rent-car_execute"),
            new RentCarArgument
                { CarId = request.CarId, RentingCompanyId = request.RentingCompanyId, Days = request.RentCarDays });

        builder.AddActivity("CreateVacationTicket",
            new Uri("queue:create-vacation-ticket_execute"),
            new CreateVacationTicketArgument
            {
                FlightId = request.FlightId, HotelId = request.HotelId, RoomId = request.RoomId, CarId = request.CarId,
                RentingCompanyId = request.RentingCompanyId
            });

        builder.AddSubscription(
            new Uri("queue:routing-slip-event"),
            RoutingSlipEvents.All, RoutingSlipEventContents.None);

        var routingSlip = builder.Build();

        await _bus.Execute(routingSlip);

        return Accepted();
    }
}