using FlightService.Contracts.FlightReservation;
using FlightService.Infrastructure.Requests.CreateFlightReservation;
using FlightService.Infrastructure.Requests.DeleteFlightReservation;
using MassTransit;
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
        var flightReservation =
            await _mediator.Send(
                new CreateFlightReservationRequest(context.Arguments.SeatId, context.Arguments.FlightId));
        return flightReservation is null
            ? context.Faulted()
            : context.Completed(new FlightReservationLog { ReservationId = Guid.Parse(flightReservation.Id) });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<FlightReservationLog> context)
    {
        await _mediator.Send(new DeleteFlightReservationRequest(context.Log.ReservationId));
        return context.Compensated();
    }
}