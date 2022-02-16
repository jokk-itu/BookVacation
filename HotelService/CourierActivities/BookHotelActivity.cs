using CarService.Contracts.BookHotelActivity;
using HotelService.Requests;
using HotelService.Requests.CreateRentHotel;
using HotelService.Requests.DeleteRentHotel;
using MassTransit;
using MassTransit.Courier;
using MediatR;

namespace HotelService.CourierActivities;

public class BookHotelActivity : IActivity<BookHotelArgument, BookHotelLog>
{
    private readonly ILogger<BookHotelActivity> _logger;
    private readonly IMediator _mediator;

    public BookHotelActivity(ILogger<BookHotelActivity> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<BookHotelArgument> context)
    {
        var roomId = context.Arguments.RoomId;
        var hotelId = context.Arguments.HotelId;
        var days = context.Arguments.Days;
        var rentId = NewId.NextGuid();

        var result = await _mediator.Send(new CreateRentHotelRequest(hotelId, roomId, days, rentId));
        return result == RequestResult.Ok
            ? context.Completed(new { RentId = rentId })
            : context.Faulted();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BookHotelLog> context)
    {
        var result = await _mediator.Send(new DeleteRentHotelRequest(context.Log.RentId));
        return result == RequestResult.Ok
            ? context.Compensated()
            : context.Failed();
    }
}