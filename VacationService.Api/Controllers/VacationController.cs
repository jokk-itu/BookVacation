using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Microsoft.AspNetCore.Mvc;
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
            new { request.FlightId, request.SeatId });

        builder.AddActivity("BookHotel",
            new Uri("queue:book-hotel_execute"),
            new { request.HotelId, Days = request.RentHotelDays, request.RoomId });

        builder.AddActivity("RentCar",
            new Uri("queue:rent-car_execute"),
            new { request.CarId, request.RentingCompanyId, Days = request.RentCarDays });

        builder.AddSubscription(
            new Uri("queue:routing-slip-event"),
            RoutingSlipEvents.All, RoutingSlipEventContents.None);

        var routingSlip = builder.Build();

        await _bus.Execute(routingSlip);

        return Accepted();
    }
}