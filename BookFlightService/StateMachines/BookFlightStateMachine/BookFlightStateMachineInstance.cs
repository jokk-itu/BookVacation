using System;
using Automatonymous;
using MassTransit.Saga;

namespace BookFlightService.StateMachines.BookFlightStateMachine;

public class BookFlightStateMachineInstance : SagaStateMachineInstance, ISagaVersion
{
    public int CurrentState { get; set; }
    public Guid? ExpirationDurationToken { get; set; }
    public int Version { get; set; }
    public Guid CorrelationId { get; set; }
}