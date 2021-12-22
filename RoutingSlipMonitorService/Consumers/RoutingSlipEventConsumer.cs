using MassTransit;
using MassTransit.Courier.Contracts;

namespace RoutingSlipMonitorService.Consumers;

public class RoutingSlipEventConsumer : 
    IConsumer<RoutingSlipCompleted>,
    IConsumer<RoutingSlipFaulted>,
    IConsumer<RoutingSlipCompensationFailed>,
    IConsumer<RoutingSlipActivityCompensated>,
    IConsumer<RoutingSlipActivityCompleted>,
    IConsumer<RoutingSlipActivityFaulted>,
    IConsumer<RoutingSlipActivityCompensationFailed>
{
    private readonly ILogger<RoutingSlipEventConsumer> _logger;

    public RoutingSlipEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
    {
        await LogPerformance(context, context.Message.Duration);
    }

    public async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
    {
        await LogPerformance(context, context.Message.Duration);
    }

    public async Task Consume(ConsumeContext<RoutingSlipCompensationFailed> context)
    {
        await LogPerformance(context, context.Message.Duration);
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityCompensated> context)
    {
        await LogPerformance(context, context.Message.Duration);
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
    {
        await LogPerformance(context, context.Message.Duration);
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
    {
        await LogPerformance(context, context.Message.Duration);
    }

    public async Task Consume(ConsumeContext<RoutingSlipActivityCompensationFailed> context)
    {
        await LogPerformance(context, context.Message.Duration);
    }

    private Task LogPerformance<T>(ConsumeContext<T> context, TimeSpan elapsed) where T: class
    {
        _logger.LogInformation("{RoutingSlipEvent} took {Elapsed} ms", context.Message.GetType().Name, elapsed);
        return Task.CompletedTask;
    } 
}