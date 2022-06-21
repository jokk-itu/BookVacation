using CarService.Contracts.RentalDeal;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using CarService.Infrastructure.Requests.DeleteRentalDeal;
using MassTransit;
using Mediator;
using MediatR;

namespace CarService.Infrastructure.CourierActivities;

public class RentalDealActivity : IActivity<RentalDealArgument, RentalDealLog>
{
    private readonly IMediator _mediator;

    public RentalDealActivity(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<RentalDealArgument> context)
    {
        var response = await _mediator.Send(new CreateRentalDealCommand(
            context.Arguments.RentFrom,
            context.Arguments.RentTo,
            context.Arguments.RentalCarId));

        if (response.ResponseCode != ResponseCode.Ok)
            return context.Faulted();

        return context.Completed(new { RentalDealId = response.Body!.Id });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<RentalDealLog> context)
    {
        var response = await _mediator.Send(new DeleteRentalDealCommand(context.Log.RentalDealId));

        return response.ResponseCode != ResponseCode.Ok ? context.Failed() : context.Compensated();
    }
}