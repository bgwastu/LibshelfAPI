using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using LibshelfAPI.Features.Books;
using LibshelfAPI.Features.Users;
using LibshelfAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LibshelfAPI.Test;

public class UsersControllerTest : TestWithSqlite
{
    
    [Fact]
    public async void Register_Returns_User_Response()
    {
        // Arrange
        var controller = new UsersController(Context, GetConfiguration());
        
        // Act
        var result = await controller.Register(new UserRegister("John Doe", "john@doe.com", "123qwe123"));
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UserResponse>((result as OkObjectResult)?.Value);
    }

    [Fact]
    public async void Login_Returns_User_Response()
    {
        // Arrange
        var controller = new UsersController(Context, GetConfiguration());
        
        // Act
        var result = await controller.Login(new UserLogin("john@smith.com", "password"));
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UserResponse>((result as OkObjectResult)?.Value);
    }

    private static IConfiguration GetConfiguration()
    {
        // JWT Mock configuration
        var jwtSettings = new Mock<IConfiguration>();
        jwtSettings.Setup(x => x["Jwt:Key"]).Returns("92745BBD-6CC8-4634-A9C1-825FF8153A1A");
        jwtSettings.Setup(x => x["Jwt:Issuer"]).Returns("issuer");
        jwtSettings.Setup(x => x["Jwt:Audience"]).Returns("audience");
        
        return jwtSettings.Object;
    }
}