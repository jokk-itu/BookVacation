using HotelService.Contracts.HotelRoomReservationActivity;
using HotelService.Infrastructure.Requests.CreateHotelRoomReservation;
using HotelService.Infrastructure.Requests.DeleteHotelRoomReservation;
using MassTransit;
using Mediator;
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
        var response = await _mediator.Send(new CreateHotelRoomReservationCommand(context.Arguments.HotelId,
            context.Arguments.RoomId, context.Arguments.From, context.Arguments.To));

        if (response.ResponseCode != ResponseCode.Ok)
            return context.Faulted();

        return context.Completed(new HotelRoomReservationLog
        {
            HotelRoomReservationId = Guid.Parse(response.Body!.Id)
        });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<HotelRoomReservationLog> context)
    {
        var response = await _mediator.Send(new DeleteHotelRoomReservationCommand(context.Log.HotelRoomReservationId));
        return response.ResponseCode != ResponseCode.Ok ? context.Failed() : context.Compensated();
    }
}