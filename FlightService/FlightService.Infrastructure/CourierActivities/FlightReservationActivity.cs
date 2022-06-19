using FlightService.Contracts.FlightReservation;
using FlightService.Infrastructure.Requests.CreateFlightReservation;
using FlightService.Infrastructure.Requests.DeleteFlightReservation;
using MassTransit;
using Mediator;
using MediatR;

namespace FlightService.Infrastructure.CourierActivities;

public class FlightReservationActivity : IActivity<FlightReservationArgument, FlightReservationLog>
{
    private readonly IMediator _mediator;

    public FlightReservationActivity(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<FlightReservationArgument> context)
    {
        var response =
            await _mediator.Send(
                new CreateFlightReservationCommand(context.Arguments.SeatId, context.Arguments.FlightId));
        return response.ResponseCode != ResponseCode.Ok
            ? context.Faulted()
            : context.Completed(new FlightReservationLog { ReservationId = Guid.Parse(response.Body!.Id) });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<FlightReservationLog> context)
    {
        await _mediator.Send(new DeleteFlightReservationCommand(context.Log.ReservationId));
        return context.Compensated();
    }
}