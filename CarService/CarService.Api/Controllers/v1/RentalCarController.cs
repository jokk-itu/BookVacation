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
    public async Task<IActionResult> Post([FromBody] PostRentalCarRequest request)
    {
        var rentalCar = await _mediator.Send(new CreateRentalCarRequest(request.CarModelNumber, request.CarCompanyName, request.RentingCompanyName, request.DayPrice, request.Color));
        return Created("", new PostRentalCarResponse
        {
            Id = rentalCar.Id,
            CarModelNumber = rentalCar.CarModelNumber,
            CarCompanyName = rentalCar.CarCompanyName,
            RentingCompanyName = rentalCar.RentalCompanyName,
            DayPrice = rentalCar.DayPrice,
            Color = rentalCar.Color
        });
    }
}