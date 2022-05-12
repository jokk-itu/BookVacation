using CarService.Contracts.RentalDeal;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using CarService.Infrastructure.Requests.DeleteRentalDeal;
using MassTransit;
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
        var rentalDeal = await _mediator.Send(new CreateRentalDealRequest(
            context.Arguments.RentFrom,
            context.Arguments.RentTo,
            context.Arguments.RentalCarId));

        return rentalDeal is null ? context.Faulted() : context.Completed(new { RentalDealId = rentalDeal.Id });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<RentalDealLog> context)
    {
        await _mediator.Send(new DeleteRentalDealRequest(context.Log.RentalDealId));
        return context.Compensated();
    }
}