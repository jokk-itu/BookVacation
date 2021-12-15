using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts;
using Contracts.Vacation;
using MassTransit;
using MassTransit.Courier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [ControllerName("vacation")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class VacationController : ControllerBase
    {
        private readonly IBusControl _bus;
        private readonly ILogger<VacationController> _logger;

        public VacationController(IBusControl bus, ILogger<VacationController> logger)
        {
            _bus = bus;
            _logger = logger;
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
                new { CarId = request.RentCarId, request.RentingCompanyId, Days = request.RentCarDays });
            
            var routingSlip = builder.Build();

            await _bus.Execute(routingSlip);
            
            return Accepted();
        }
    }
}