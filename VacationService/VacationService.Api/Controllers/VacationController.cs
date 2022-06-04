using Microsoft.AspNetCore.Mvc;
using VacationService.Contracts.Vacation;
using MediatR;
using VacationService.Infrastructure.Requests.CreateVacation;

namespace VacationService.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[ControllerName("vacation")]
[Route("api/v{version:apiVersion}/[controller]")]
public class VacationController : ControllerBase
{
    private readonly IMediator _mediator;

    public VacationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] PostVacationRequest request, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new CreateVacationRequest(request.FlightId, request.FlightSeatId, request.HotelId,
            request.HotelFrom, request.HotelTo, request.HotelRoomId, request.RentalCarId, request.RentingCompanyName,
            request.RentalCarFrom, request.RentalCarTo), cancellationToken);
        return Accepted();
    }
}