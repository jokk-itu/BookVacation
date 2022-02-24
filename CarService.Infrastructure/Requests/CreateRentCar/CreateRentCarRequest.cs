using MediatR;

namespace CarService.Infrastructure.Requests.CreateRentCar;

public record CreateRentCarRequest(Guid CompanyId, Guid CarId, uint Days, Guid RentId) : IRequest<RequestResult>;