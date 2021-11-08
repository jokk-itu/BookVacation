using Automatonymous;
using Contracts;

namespace BookFlightService.StateMachines.BookFlightStateMachine
{
    public partial class BookFlightStateMachine
    {
        private Event<CreateBookFlight> CreateBookFlight { get; set; }

        private Event<CancelBookFlight> CancelBookFlight { get; set; }

        private Event<CompleteBookFlight> CompleteBookFlight { get; set; }

        private Event<ExpireBookFlight> ExpireBookFlight { get; set; }

        private void SetupEvents()
        {
            Event(() => CreateBookFlight,
                x => x.CorrelateById(c => c.Message.BookFlightId));

            Event(() => CancelBookFlight,
                x => x.CorrelateById(c => c.Message.BookFlightId));

            Event(() => CompleteBookFlight,
                x => x.CorrelateById(c => c.Message.BookFlightId));

            Event(() => ExpireBookFlight,
                x => x.CorrelateById(c => c.Message.BookFlightId));
        }
    }
}