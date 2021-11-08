using System;
using Automatonymous;
using Contracts;

namespace BookFlightService.StateMachines.BookFlightStateMachine
{
    public partial class BookFlightStateMachine
    {
        private Schedule<BookFlightStateMachineInstance, ExpireBookFlight> BookFlightExpiredSchedule { get; set; }
        
        private void SetupScheduling()
        {
            Schedule(
                () => BookFlightExpiredSchedule, 
                x => x.ExpirationDurationToken, 
                c => c.Delay = TimeSpan.FromSeconds(10));
        }
    }
}