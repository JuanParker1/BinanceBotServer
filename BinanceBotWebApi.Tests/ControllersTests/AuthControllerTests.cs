using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authService = new Mock<IAuthService>();
        
        _controller = new AuthController(_authService.Object);
        
        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_if_wrong_credentials()
    {
        _authService.Setup(s => s.LoginAsync(It.IsAny<string>(), 
                It.IsAny<string>(), CancellationToken.None))
            .Returns(Task.FromResult<AuthTokenDto>(null));
        
        var result = _controller.LoginAsync(new AuthDto())
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_if_right_credentials()
    {
        _authService.Setup(s => s.LoginAsync(It.IsAny<string>(),
                It.IsAny<string>(), CancellationToken.None).Result)
            .Returns(new AuthTokenDto());

        var result = _controller.LoginAsync(new AuthDto())
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_new_token_in_refresh()
    {
        _authService.Setup(s => s.Refresh(It.IsAny<ClaimsPrincipal>()))
            .Returns("new token");
        
        var result = _controller.Refresh();
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_if_user_already_exists()
    {
        _authService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>(),
                CancellationToken.None).Result)
            .Returns(false);

        var result = _controller.RegisterAsync(new RegisterDto())
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_if_new_user_credentials()
    {
        _authService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>(),
                CancellationToken.None).Result)
            .Returns(true);
        
        _authService.Setup(s => s.LoginAsync(It.IsAny<string>(),
                It.IsAny<string>(), CancellationToken.None).Result)
            .Returns(new AuthTokenDto());
        
        var result = _controller.RegisterAsync(new RegisterDto())
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
}