using Mediator;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.UpdateTracking;

public record UpdateTrackingCommand
    (string TrackingNumber, string Result, DateTimeOffset OccuredAt) : ICommand<Tracking>;