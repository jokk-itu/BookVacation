using Mediator;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.ReadTracking;

public record ReadTrackingCommand(string TrackingNumber) : ICommand<Tracking>;