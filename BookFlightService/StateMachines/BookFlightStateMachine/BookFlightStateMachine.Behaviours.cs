using System.Text.Json;
using System.Threading.Tasks;
using Automatonymous;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookFlightService.StateMachines.BookFlightStateMachine
{
    public partial class BookFlightStateMachine
    {
        private void SetupBehaviours()
        {
            Initially(When(CreateBookFlight)
                    .Schedule(BookFlightExpiredSchedule,
                        context => context.Init<ExpireBookFlight>(new
                            {
                                BookFlightId = context.Data.FlightId, context.Data.Price }))
                    .ThenAsync(context =>
                    {
                        _logger.LogInformation("From initial to create book flight {Message}",
                            JsonSerializer.Serialize(context.Data));
                        return Task.CompletedTask;
                    })
                    .TransitionTo(Pending),
                When(CancelBookFlight)
                    .ThenAsync(context =>
                    {
                        _logger.LogInformation("From initial to cancel book flight {Message}",
                            JsonSerializer.Serialize(context.Data));
                        return Task.CompletedTask;
                    })
                    .TransitionTo(Cancelled)
                    .Finalize(),
                Ignore(BookFlightExpiredSchedule.Received));

            During(Pending,
                When(CompleteBookFlight)
                    .ThenAsync(context =>
                    {
                        _logger.LogInformation("From pending to complete book flight {Message}",
                            JsonSerializer.Serialize(context.Data));
                        return Task.CompletedTask;
                    })
                    .Finalize(),
                When(CancelBookFlight)
                    .ThenAsync(context =>
                    {
                        _logger.LogInformation("From pending to cancel book flight {Message}",
                            JsonSerializer.Serialize(context.Data));
                        return Task.CompletedTask;
                    })
                    .TransitionTo(Cancelled)
                    .Finalize(),
                Ignore(CreateBookFlight),
                When(ExpireBookFlight)
                    .ThenAsync(context =>
                    {
                        _logger.LogInformation("From pending to expired book flight {Message}",
                            JsonSerializer.Serialize(context.Data));
                        return Task.CompletedTask;
                    })
                    .TransitionTo(Cancelled)
                    .Finalize());

            SetCompletedWhenFinalized();
        }
    }
}