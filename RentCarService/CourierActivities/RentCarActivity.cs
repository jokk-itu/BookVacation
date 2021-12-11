using System;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.RentCarActivity;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;

namespace RentCarService.CourierActivities;

public class RentCarActivity : IActivity<RentCarArgument, RentCarLog>
{
    private readonly ILogger<RentCarActivity> _logger;

    public RentCarActivity(ILogger<RentCarActivity> logger)
    {
        _logger = logger;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<RentCarArgument> context)
    {
        _logger.LogInformation("Executing RentCar");
        var price = context.Arguments.Price;
        var rentCarId = context.Arguments.CarId;

        _logger.LogInformation("Executed RentCar");
        return context.Completed();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<RentCarLog> context)
    {
        await Task.Delay(500);
        _logger.LogInformation("RentCar Compensated {Log}", JsonSerializer.Serialize(context.Log));
        return context.Compensated();
    }
}