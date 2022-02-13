using Contracts.BookFlightActivity;
using FlightService.Requests;
using FlightService.Requests.CreateReservation;
using FlightService.Requests.DeleteReservation;
using MassTransit;
using MassTransit.Courier;
using MediatR;

namespace FlightService.CourierActivities;

public class BookFlightActivity : IActivity<BookFlightArgument, BookFlightLog>
{
    private readonly IMediator _mediator;

    public BookFlightActivity(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<BookFlightArgument> context)
    {
        var reservationId = NewId.NextGuid();
        var result =
            await _mediator.Send(new CreateReservationRequest(context.Arguments.SeatId, context.Arguments.FlightId,
                reservationId));
        return result == RequestResult.Ok
            ? context.Completed(new { ReservationId = reservationId })
            : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BookFlightLog> context)
    {
        var reservationId = context.Log.ReservationId;
        var result = await _mediator.Send(new DeleteReservationRequest(reservationId));
        return result == RequestResult.Ok
            ? context.Compensated()
            : context.Failed();
    }
}