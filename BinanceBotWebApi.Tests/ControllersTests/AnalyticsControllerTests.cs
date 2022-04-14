using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data.Analytics;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class AnalyticsControllerTests
{
    private readonly Mock<IAnalyticsService> _analyticsService;
    private readonly AnalyticsController _controller;

    public AnalyticsControllerTests()
    {
        _analyticsService = new Mock<IAnalyticsService>();
        
        _controller = new AnalyticsController(_analyticsService.Object);
        
        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_profit_to_btc_if_wrong_user_id()
    {
        var result = _controller.GetProfitToBtcAsync(0, DateTime.Now, DateTime.Now)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_profit_to_btc_if_dtos_is_not_null()
    {
        _analyticsService.Setup(s => s.GetProfitToBtcAsync(It.IsAny<int>(), 
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), 
                CancellationToken.None).Result)
            .Returns(new ProfitToBtcDto());

        var result = _controller.GetProfitToBtcAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_profit_to_btc_if_dtos_is_null()
    {
        _analyticsService.Setup(s => s.GetProfitToBtcAsync(It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<DateTime>(), CancellationToken.None))
            .Returns(Task.FromResult<ProfitToBtcDto>(null));

        var result = _controller.GetProfitToBtcAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_trade_types_stats_if_wrong_user_id()
    {
        var result = _controller.GetTradeTypesStatsAsync(0, DateTime.Now, DateTime.Now)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_trade_types_stats_if_dtos_is_not_null()
    {
        _analyticsService.Setup(s => s.GetTradeTypesStatsAsync(It.IsAny<int>(), 
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), 
                CancellationToken.None).Result)
            .Returns(new TradeTypesStatsDto());

        var result = _controller.GetTradeTypesStatsAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_trade_types_stats_if_dtos_is_null()
    {
        _analyticsService.Setup(s => s.GetTradeTypesStatsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<DateTime>(), CancellationToken.None))
            .Returns(Task.FromResult<TradeTypesStatsDto>(null));

        var result = _controller.GetProfitToBtcAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_profit_details_if_wrong_user_id()
    {
        var result = _controller.GetProfitDetailsAsync(0, DateTime.Now, DateTime.Now)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_profit_details_if_dtos_is_not_null()
    {
        _analyticsService.Setup(s => s.GetProfitDetailsAsync(It.IsAny<int>(), 
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), 
                CancellationToken.None).Result)
            .Returns(new List<ProfitDetailsDto>());

        var result = _controller.GetProfitDetailsAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_profit_details_if_dtos_is_null()
    {
        _analyticsService.Setup(s => s.GetProfitDetailsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<DateTime>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<ProfitDetailsDto>>(null));

        var result = _controller.GetProfitToBtcAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
}