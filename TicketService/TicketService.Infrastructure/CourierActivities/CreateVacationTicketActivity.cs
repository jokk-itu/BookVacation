using MassTransit;
using Mediator;
using MediatR;
using TicketService.Contracts.CreateVacationTickets;
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
        var carTicketResponse =
            await _mediator.Send(
                new CreateCarTicketCommand(context.Arguments.CarId, context.Arguments.RentingCompanyName));
        var hotelTicketResponse =
            await _mediator.Send(new CreateHotelTicketCommand(context.Arguments.HotelId, context.Arguments.RoomId));
        var flightTicketResponse = await _mediator.Send(new CreateFlightTicketCommand(context.Arguments.FlightId));

        if (carTicketResponse.ResponseCode == ResponseCode.Ok
            && hotelTicketResponse.ResponseCode == ResponseCode.Ok
            && flightTicketResponse.ResponseCode == ResponseCode.Ok)
            return context.Completed();

        return context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<CreateVacationTicketLog> context)
    {
        await Task.Yield();
        return context.Compensated();
    }
}