using Automatonymous;

namespace BookFlightService.StateMachines.BookFlightStateMachine
{
    public partial class BookFlightStateMachine
    {
        private State Created { get; set; }
        private State Pending { get; set; }
        private State Cancelled { get; set; }

        private void SetupStates()
        {
            InstanceState(x =>
                x.CurrentState, Created, Pending, Cancelled);
        }
    }
}