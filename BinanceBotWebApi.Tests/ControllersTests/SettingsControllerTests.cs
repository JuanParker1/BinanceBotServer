using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class SettingsControllerTests
{
    private readonly Mock<ISettingsService> _settingsService;
    private readonly SettingsController _controller;

    public SettingsControllerTests()
    {
        _settingsService = new Mock<ISettingsService>();

        _controller = new SettingsController(_settingsService.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_user_settings_if_wrong_user_id()
    {
        var result = _controller.GetUserSettings(0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_user_settings_if_dtos_is_not_null()
    {
        _settingsService.Setup(s => s.GetSettingsAsync(It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(new SettingsDto());

        var result = _controller.GetUserSettings(1).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_user_settings_if_dtos_is_null()
    {
        _settingsService.Setup(s => s.GetSettingsAsync(It.IsAny<int>(),
                CancellationToken.None))
            .Returns(Task.FromResult<SettingsDto>(null));

        var result = _controller.GetUserSettings(1).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_switch_trade_if_wrong_user_id()
    {
        var result = _controller.SwitchTradeAsync(new SwitchTradeDto {IdUser = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_switch_trade()
    {
        _settingsService.Setup(s => s.SwitchTradeAsync(It.IsAny<SwitchTradeDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.SwitchTradeAsync(new SwitchTradeDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_with_correct_value_in_switch_trade()
    {
        _settingsService.Setup(s => s.SwitchTradeAsync(It.IsAny<SwitchTradeDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.SwitchTradeAsync(new SwitchTradeDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
        
        Assert.Equal(1,okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_save_trade_mode_if_wrong_user_id()
    {
        var result = _controller.SaveTradeModeAsync(new TradeModeDto {IdUser = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_save_trade_mode()
    {
        _settingsService.Setup(s => s.SaveTradeModeAsync(It.IsAny<TradeModeDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.SaveTradeModeAsync(new TradeModeDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_with_correct_value_in_save_trade_mode()
    {
        _settingsService.Setup(s => s.SaveTradeModeAsync(It.IsAny<TradeModeDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.SaveTradeModeAsync(new TradeModeDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
        
        Assert.Equal(1,okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_change_order_price_rate_if_wrong_user_id()
    {
        var result = _controller.ChangeOrderPriceRateAsync(new OrderPriceRateDto {IdUser = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_change_order_price_rate()
    {
        _settingsService.Setup(s => s.ChangeOrderPriceRateAsync(It.IsAny<OrderPriceRateDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.ChangeOrderPriceRateAsync(new OrderPriceRateDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_with_correct_value_in_change_order_price_rate()
    {
        _settingsService.Setup(s => s.ChangeOrderPriceRateAsync(It.IsAny<OrderPriceRateDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.ChangeOrderPriceRateAsync(new OrderPriceRateDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
        
        Assert.Equal(1,okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_save_api_keys_if_wrong_user_id()
    {
        var result = _controller.SaveApiKeysAsync(new ApiKeysDto {IdUser = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_save_api_keys()
    {
        _settingsService.Setup(s => s.SaveApiKeysAsync(It.IsAny<ApiKeysDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.SaveApiKeysAsync(new ApiKeysDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_with_correct_value_in_save_api_keys()
    {
        _settingsService.Setup(s => s.SaveApiKeysAsync(It.IsAny<ApiKeysDto>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.SaveApiKeysAsync(new ApiKeysDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
        
        Assert.Equal(1,okObjectResult?.Value);
    }
}