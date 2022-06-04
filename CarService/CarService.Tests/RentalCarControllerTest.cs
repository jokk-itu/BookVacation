using System.Net;
using System.Net.Http.Json;
using CarService.Api;
using CarService.Contracts.RentalCar;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CarService.Tests;

public class RentalCarControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task Post_ExpectCreated()
    {
        //Arrange
        
        //Act
        
        //Assert
    }
}