using CarService.Contracts.RentalCar;
using CarService.Infrastructure.Requests.CreateRentalCar;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class RentalCarController : ControllerBase
{
    private readonly IMediator _mediator;

    public RentalCarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PostRentalCarResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] PostRentalCarRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _mediator.Send(new CreateRentalCarCommand(request.CarModelNumber, request.CarCompanyName,
            request.RentingCompanyName, request.DayPrice, request.Color), cancellationToken);

        return Created("", new PostRentalCarResponse
        {
            Id = Guid.Parse(response.Body!.Id),
            CarModelNumber = response.Body!.CarModelNumber,
            CarCompanyName = response.Body!.CarCompanyName,
            RentingCompanyName = response.Body!.RentalCompanyName,
            DayPrice = response.Body!.DayPrice,
            Color = response.Body!.Color
        });
    }
}