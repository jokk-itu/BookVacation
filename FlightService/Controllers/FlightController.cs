using Microsoft.AspNetCore.Mvc;

namespace FlightService.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FlightController : ControllerBase
{
    public FlightController()
    {}

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetFlightAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetFlightsAsync(
        [FromQuery] uint amount, 
        [FromQuery] uint offset = 0, 
        CancellationToken cancellationToken = default)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> PostFlightAsync(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}