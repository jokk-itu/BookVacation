using Automatonymous;
using Contracts.BookFlightStateMachine;

namespace FlightService.StateMachines.BookFlightStateMachine;

public partial class BookFlightStateMachine
{
    private Event<CreateBookFlight> CreateBookFlight { get; set; }

    private Event<CancelBookFlight> CancelBookFlight { get; set; }

    private Event<CompleteBookFlight> CompleteBookFlight { get; set; }

    private Event<ExpireFlight> ExpireBookFlight { get; set; }

    private void SetupEvents()
    {
        Event(() => CreateBookFlight,
            x => x.CorrelateById(c => c.Message.FlightId));

        Event(() => CancelBookFlight,
            x => x.CorrelateById(c => c.Message.FlightId));

        Event(() => CompleteBookFlight,
            x => x.CorrelateById(c => c.Message.FlightId));

        Event(() => ExpireBookFlight,
            x => x.CorrelateById(c => c.Message.FlightId));
    }
}