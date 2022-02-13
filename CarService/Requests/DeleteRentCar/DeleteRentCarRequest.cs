using MediatR;

namespace CarService.Requests.DeleteRentCar;

public record DeleteRentCarRequest(Guid RentCarId) : IRequest<RequestResult>;