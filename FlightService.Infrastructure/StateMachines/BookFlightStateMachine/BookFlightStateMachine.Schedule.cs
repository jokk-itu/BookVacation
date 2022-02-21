using Automatonymous;
using FlightService.Contracts.BookFlightStateMachine;

namespace FlightService.Infrastructure.StateMachines.BookFlightStateMachine;

public partial class BookFlightStateMachine
{
    private Schedule<BookFlightStateMachineInstance, ExpireFlight> BookFlightExpiredSchedule { get; set; }

    private void SetupScheduling()
    {
        Schedule(
            () => BookFlightExpiredSchedule,
            x => x.ExpirationDurationToken,
            c => { c.Delay = TimeSpan.FromSeconds(7); });
    }
}