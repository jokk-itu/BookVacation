using System;

namespace Contracts;

public record RoutingSlipCreated
{
    public Guid TrackingNumber { get; init; }
    
    public DateTime Timestamp { get; init; }
}