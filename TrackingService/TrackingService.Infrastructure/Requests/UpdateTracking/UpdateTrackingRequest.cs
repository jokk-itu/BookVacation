using MediatR;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.UpdateTracking;

public record UpdateTrackingRequest(string TrackingNumber, string Result, DateTimeOffset OccuredAt) : IRequest<Tracking>;