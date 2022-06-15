using MassTransit;
using MediatR;
using TicketService.Contracts.CreateVacationTickets;
using TicketService.Infrastructure.Requests;
using TicketService.Infrastructure.Requests.CreateCarTicket;
using TicketService.Infrastructure.Requests.CreateFlightTicket;
using TicketService.Infrastructure.Requests.CreateHotelTicket;

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
        var carTicketResult =
            await _mediator.Send(
                new CreateCarTicketRequest(context.Arguments.CarId, context.Arguments.RentingCompanyName));
        var hotelTicketResult =
            await _mediator.Send(new CreateHotelTicketRequest(context.Arguments.HotelId, context.Arguments.RoomId));
        var flightTicketResult = await _mediator.Send(new CreateFlightTicketRequest(context.Arguments.FlightId));
        if (carTicketResult == RequestResult.Created
            && hotelTicketResult == RequestResult.Created
            && flightTicketResult == RequestResult.Created)
            return context.Completed();

        return context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<CreateVacationTicketLog> context)
    {
        await Task.Yield();
        return context.Compensated();
    }
}