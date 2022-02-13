using MediatR;

namespace CarService.Requests.CreateRentCar;

public record CreateRentCarRequest(Guid CompanyId, Guid CarId, uint Days, Guid RentId) : IRequest<RequestResult>;