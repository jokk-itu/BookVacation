using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [ControllerName("bookvacation")]
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
            _logger.LogInformation("Sending Book Vacation {Message}", JsonSerializer.Serialize(request));
            var endpoint = await _bus.GetSendEndpoint(new Uri("exchange:book-vacation"));
            await endpoint.Send<BookVacation>(new
            {
                BookFlight = new { request.FlightId, Price = request.FlightPrice },
                BookHotel = new { request.HotelId, Price = request.HotelPrice },
                RentCar = new { request.RentCarId, Price = request.CarPrice }
            });

            return Accepted();
        }
    }
}