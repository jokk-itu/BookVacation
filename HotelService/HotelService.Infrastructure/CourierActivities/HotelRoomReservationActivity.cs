using HotelService.Contracts.HotelRoomReservationActivity;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;
using MassTransit;
using MassTransit.Courier;
using MediatR;

namespace HotelService.Infrastructure.CourierActivities;

public class HotelRoomReservationActivity : IActivity<HotelRoomReservationArgument, HotelRoomReservationLog>
{
    private readonly IMediator _mediator;

    public HotelRoomReservationActivity(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<HotelRoomReservationArgument> context)
    {
        var hotelRoomReservation = await _mediator.Send(new CreateHotelRoomReservationRequest(context.Arguments.HotelId,
            context.Arguments.RoomId, context.Arguments.From, context.Arguments.To));
        return hotelRoomReservation is null
            ? context.Faulted()
            : context.Completed(new HotelRoomReservationLog { HotelRoomReservationId = Guid.Parse(hotelRoomReservation.Id) });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<HotelRoomReservationLog> context)
    {
        await _mediator.Send(new DeleteHotelRoomReservationRequest(context.Log.HotelRoomReservationId));
        return context.Compensated();
    }
}