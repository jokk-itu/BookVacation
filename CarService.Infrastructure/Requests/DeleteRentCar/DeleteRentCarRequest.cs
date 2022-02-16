using MediatR;

namespace CarService.Infrastructure.Requests.DeleteRentCar;

public record DeleteRentCarRequest(Guid RentCarId) : IRequest<RequestResult>;