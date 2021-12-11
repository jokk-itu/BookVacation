using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts;
using Contracts.BookHotelActivity;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace BookHotelService.CourierActivities;

public class BookHotelActivity : IActivity<BookHotelArgument, BookHotelLog>
{
    private readonly ILogger<BookHotelActivity> _logger;

    public BookHotelActivity(ILogger<BookHotelActivity> logger)
    {
        _logger = logger;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<BookHotelArgument> context)
    {
        _logger.LogInformation("Executing BookHotel");
        var price = context.Arguments.Price;
        var bookHotelId = context.Arguments.HotelId;

        _logger.LogInformation("Executed BookHotel");
        return context.Completed();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BookHotelLog> context)
    {
        await Task.Delay(500);
        _logger.LogInformation("RentCar Compensated {Log}", JsonSerializer.Serialize(context.Log));
        return context.Compensated();
    }
}