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
    [Route("v{version:apiVersion}/[controller]")]
    public class BookVacationController : ControllerBase
    {
        private readonly IBusControl _bus;
        private readonly ILogger<BookVacationController> _logger;

        public BookVacationController(IBusControl bus, ILogger<BookVacationController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BookVacationRequest request)
        {
            _logger.LogInformation("Sending Book Vacation {Message}", JsonSerializer.Serialize(request));
            var endpoint = await _bus.GetSendEndpoint(new Uri("exchange:book-vacation"));
            await endpoint.Send<BookVacation>(new
            {
                BookFlight = new { request.BookFlightId, Price = request.FlightPrice },
                BookHotel = new { request.BookHotelId, Price = request.HotelPrice },
                RentCar = new { request.RentCarId, Price = request.RentCarId }
            });

            return Accepted();
        }
    }
}