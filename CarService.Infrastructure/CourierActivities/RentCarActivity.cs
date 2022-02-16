using CarService.Infrastructure.Requests;
using CarService.Infrastructure.Requests.CreateRentCar;
using CarService.Infrastructure.Requests.DeleteRentCar;
using Contracts.RentCarActivity;
using MassTransit;
using MassTransit.Courier;
using MediatR;

namespace CarService.Infrastructure.CourierActivities;

public class RentCarActivity : IActivity<RentCarArgument, RentCarLog>
{
    private readonly IMediator _mediator;

    public RentCarActivity(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<RentCarArgument> context)
    {
        var companyId = context.Arguments.RentingCompanyId;
        var carId = context.Arguments.CarId;
        var days = context.Arguments.Days;
        var rentId = NewId.NextGuid();

        var result = await _mediator.Send(new CreateRentCarRequest(companyId, carId, days, rentId));
        return result == RequestResult.Ok
            ? context.Completed(new { RentId = rentId })
            : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<RentCarLog> context)
    {
        var rentId = context.Log.RentCarId;
        var result = await _mediator.Send(new DeleteRentCarRequest(rentId));
        return result == RequestResult.Ok
            ? context.Compensated()
            : context.Failed();
    }
}