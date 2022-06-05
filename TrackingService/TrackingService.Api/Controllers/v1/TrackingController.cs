using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackingService.Contracts;
using TrackingService.Infrastructure.Requests.ReadTracking;

namespace TrackingService.Api.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TrackingController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrackingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{trackingId:guid}")]
    [ProducesResponseType(typeof(GetTrackingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAsync(Guid trackingId, CancellationToken cancellationToken = default)
    {
        var tracking = await _mediator.Send(new ReadTrackingRequest(trackingId.ToString()), cancellationToken);

        if (tracking is null)
            return NotFound();

        return Ok(new GetTrackingResponse
        {
            Id = tracking.Id,
            Statuses = tracking.Statuses.Select(x => new StatusDto
            {
                Result = x.Result,
                OccuredAt = x.OccuredAt
            })
        });
    }
}