using System.Threading.Tasks;
using Contracts.BookFlightStateMachine;
using Contracts.Flight;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers;

[ApiController]
[ControllerName("flight")]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FlightController : ControllerBase
{
    private readonly IBusControl _bus;
    private readonly ILogger<FlightController> _logger;

    public FlightController(IBusControl bus, ILogger<FlightController> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    [HttpPost]
    [Route("cancel")]
    public async Task<IActionResult> CancelAsync([FromBody] BookFlightRequest request)
    {
        var correlationId = NewId.NextGuid();
        var bookEvent = new
        {
            __CorrelationId = correlationId,
            request.FlightId,
            request.Price
        };

        await _bus.Publish<CreateBookFlight>(bookEvent);
        await Task.Delay(2000);
        await _bus.Publish<CancelBookFlight>(bookEvent);
        return Accepted();
    }

    [HttpPost]
    [Route("complete")]
    public async Task<IActionResult> CompleteAsync([FromBody] BookFlightRequest request)
    {
        var correlationId = NewId.NextGuid();
        var bookEvent = new
        {
            __CorrelationId = correlationId,
            request.FlightId,
            request.Price
        };

        await _bus.Publish<CreateBookFlight>(bookEvent);
        await Task.Delay(2000);
        await _bus.Publish<CompleteBookFlight>(bookEvent);
        return Accepted();
    }

    [HttpPost]
    [Route("expire")]
    public async Task<IActionResult> ExpireAsync([FromBody] BookFlightRequest request)
    {
        var correlationId = NewId.NextGuid();
        var bookEvent = new
        {
            __CorrelationId = correlationId,
            request.FlightId,
            request.Price
        };

        await _bus.Publish<CreateBookFlight>(bookEvent);
        return Accepted();
    }
}