using CarService.Contracts.RentCarActivity;
using CarService.Infrastructure.Requests.CreateRentalDeal;
using CarService.Infrastructure.Requests.DeleteRentalDeal;
using MassTransit.Courier;
using MediatR;

namespace CarService.Infrastructure.CourierActivities;

public class RentalDealActivity : IActivity<RentalDealArgument, RentalDeaLog>
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
        
        return rentalDeal is null ? 
            context.Faulted() : 
            context.Completed(new { RentalDealId = rentalDeal.Id });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<RentalDeaLog> context)
    {
        await _mediator.Send(new DeleteRentalDealRequest(context.Log.RentalDealId));
        return context.Compensated();
    }
}