using MediatR;
using TrackingService.Domain;

namespace TrackingService.Infrastructure.Requests.ReadTracking;

public record ReadTrackingRequest(string TrackingNumber) : IRequest<Tracking?>;