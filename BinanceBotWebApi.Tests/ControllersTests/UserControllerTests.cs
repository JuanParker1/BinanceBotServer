using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<ICoinService> _coinService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userService = new Mock<IUserService>();
        _coinService = new Mock<ICoinService>();

        _controller = new UserController(_userService.Object,
            _coinService.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_user_info_if_wrong_user_id()
    {
        var result = _controller.GetUserInfoAsync(0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_user_info_if_dtos_is_not_null()
    {
        _userService.Setup(s => s.GetUserInfoAsync(It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(new UserInfoDto());

        var result = _controller.GetUserInfoAsync(1).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_user_info_if_dtos_is_null()
    {
        _userService.Setup(s => s.GetUserInfoAsync(It.IsAny<int>(),
                CancellationToken.None))
            .Returns(Task.FromResult<UserInfoDto>(null));

        var result = _controller.GetUserInfoAsync(1).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_update_user_info_if_wrong_user_id()
    {
        var result = _controller.UpdateUserInfoAsync(new UserInfoDto {Id = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_update_user_info()
    {
        _userService.Setup(s => s.UpdateUserInfoAsync(It.IsAny<UserInfoDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.UpdateUserInfoAsync(new UserInfoDto {Id = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_with_correct_value_in_update_user_info()
    {
        _userService.Setup(s => s.UpdateUserInfoAsync(It.IsAny<UserInfoDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.UpdateUserInfoAsync(new UserInfoDto {Id = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
        
        Assert.Equal(1,okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_change_password_if_wrong_user_id()
    {
        var result = _controller.ChangePasswordAsync(new ChangePasswordDto {IdUser = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_change_password_with_correct_credentials()
    {
        _userService.Setup(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.ChangePasswordAsync(new ChangePasswordDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkResult;
    
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_500_result_in_change_password()
    {
        _userService.Setup(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDto>(), 
                CancellationToken.None).Result)
            .Returns(0);

        var result = _controller.ChangePasswordAsync(new ChangePasswordDto {IdUser = 1})
            .Result;
        var objectResult = result as ObjectResult;
    
        Assert.NotNull(objectResult);
    }
    
    [Fact]
    public void It_should_return_400_result_in_change_password_if_user_not_exists()
    {
        _userService.Setup(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDto>(), 
                CancellationToken.None).Result)
            .Returns(-1);

        var result = _controller.ChangePasswordAsync(new ChangePasswordDto {IdUser = 1})
            .Result;
        var objectResult = result as BadRequestObjectResult;
    
        Assert.NotNull(objectResult);
    }
    
    [Fact]
    public void It_should_return_400_result_in_change_password_if_old_passord_wrong()
    {
        _userService.Setup(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDto>(), 
                CancellationToken.None).Result)
            .Returns(-2);

        var result = _controller.ChangePasswordAsync(new ChangePasswordDto {IdUser = 1})
            .Result;
        var objectResult = result as BadRequestObjectResult;
    
        Assert.NotNull(objectResult);
    }
    
    [Fact]
    public void It_should_return_400_result_in_change_password_if_other_error_code()
    {
        _userService.Setup(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDto>(), 
                CancellationToken.None).Result)
            .Returns(-111);

        var result = _controller.ChangePasswordAsync(new ChangePasswordDto {IdUser = 1})
            .Result;
        var objectResult = result as BadRequestObjectResult;
    
        Assert.NotNull(objectResult);
    }
    
    [Fact]
    public void It_should_return_403_in_get_userdata_stream_if_wrong_user_id()
    {
        var result = _controller.GetUserDataStreamAsync(0, "")
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_userdata_stream()
    {
        var result = _controller.GetUserDataStreamAsync(1, "")
            .Result;
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
    
    [Fact]
    public void It_should_return_403_in_get_subscriptions_list_if_wrong_user_id()
    {
        var result = _controller.GetSubscriptionsListAsync(0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_subscriptions_list()
    {
        var result = _controller.GetSubscriptionsListAsync(1)
            .Result;
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
}