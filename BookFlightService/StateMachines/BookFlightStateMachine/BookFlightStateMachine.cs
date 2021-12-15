using Automatonymous;
using Microsoft.Extensions.Logging;

namespace BookFlightService.StateMachines.BookFlightStateMachine;

public partial class BookFlightStateMachine : MassTransitStateMachine<BookFlightStateMachineInstance>
{
    private readonly ILogger<BookFlightStateMachine> _logger;

    public BookFlightStateMachine(ILogger<BookFlightStateMachine> logger)
    {
        _logger = logger;

        SetupEvents();
        SetupStates();
        SetupScheduling();
        SetupBehaviours();
    }
}