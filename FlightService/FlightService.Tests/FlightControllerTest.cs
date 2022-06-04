using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FlightService.Api;
using FlightService.Contracts.Airplane;
using FlightService.Contracts.Flight;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlightService.Tests;

public class FlightControllerTest : IClassFixture<WebApplicationFactory<Program>>
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