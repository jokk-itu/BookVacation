using Automatonymous;
#nullable disable

namespace FlightService.StateMachines.BookFlightStateMachine;

public partial class BookFlightStateMachine : MassTransitStateMachine<BookFlightStateMachineInstance>
{
    private readonly ILogger<BookFlightStateMachine> _logger;

    // ReSharper disable once NotNullMemberIsNotInitialized
    public BookFlightStateMachine(ILogger<BookFlightStateMachine> logger)
    {
        _logger = logger;

        SetupEvents();
        SetupStates();
        SetupScheduling();
        SetupBehaviours();
    }
}