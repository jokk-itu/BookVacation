using Bogus;
using CarService.Contracts.RentalCar;

namespace TestConsole.Fakes;

public static class RentalCarFaker
{
    public static Faker<PostRentalCarRequest> GetRentalCarRequestFaker()
    {
        return new Faker<PostRentalCarRequest>().Rules((faker, request) =>
        {
            request.Color = "Blue";
            request.DayPrice = (short)Random.Shared.Next(1, 10000);
            request.CarCompanyName = faker.Company.CompanyName();
            request.RentingCompanyName = faker.Company.CompanyName();
            request.CarModelNumber = Guid.NewGuid();
        });
    }
}