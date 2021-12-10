using System;
using Automatonymous;
using Contracts;
using Contracts.BookFlightStateMachine;

namespace BookFlightService.StateMachines.BookFlightStateMachine
{
    public partial class BookFlightStateMachine
    {
        private Schedule<BookFlightStateMachineInstance, ExpireFlight> BookFlightExpiredSchedule { get; set; }

        private void SetupScheduling()
        {
            Schedule(
                () => BookFlightExpiredSchedule,
                x => x.ExpirationDurationToken,
                c =>
                {
                    c.Delay = TimeSpan.FromSeconds(7);
                });
        }
    }
}