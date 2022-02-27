using MassTransit.Courier;
using MediatR;
using TicketService.Contracts.CreateVacationTickets;
using TicketService.Infrastructure.Requests;
using TicketService.Infrastructure.Requests.CreateCarTicket;

namespace TicketService.Infrastructure.CourierActivities;

public class CreateVacationTicketActivity : IActivity<CreateVacationTicketArgument, CreateVacationTicketLog>
{
    private readonly IMediator _mediator;

    public CreateVacationTicketActivity(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<CreateVacationTicketArgument> context)
    {
        var carTicketResult = await _mediator.Send(new CreateCarTicketRequest(context.Arguments.CarId, context.Arguments.RentingCompanyId));
        //var hotelTicketResult = await _mediator.Send(new CreateHotelTicketRequest());
        //var flightTicketResult = await _mediator.Send(new CreateFlightTicketRequest());
        return carTicketResult == RequestResult.Ok ? context.Completed(new CreateVacationTicketLog()) : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<CreateVacationTicketLog> context)
    {
        return context.Compensated();
    }
}