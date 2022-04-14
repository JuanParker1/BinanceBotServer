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

public class CryptoInfoControllerTests
{
    private readonly Mock<ICryptoInfoService> _cryptoInfoService;
    private readonly CryptoInfoController _controller;

    public CryptoInfoControllerTests()
    {
        _cryptoInfoService = new Mock<ICryptoInfoService>();

        _controller = new CryptoInfoController(_cryptoInfoService.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_btc_prices_history_if_wrong_user_id()
    {
        var result = _controller.GetBtcPriceHistoryAsync(0, "", 
                DateTime.Now, DateTime.Now).Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_btc_prices_history_if_dtos_is_not_null()
    {
        _cryptoInfoService.Setup(s => s.GetPriceHistoryAsync(It.IsAny<string>(), 
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), CancellationToken.None)
                .Result)
            .Returns(new List<ProfitToBtcHistoryDto>());

        var result = _controller.GetBtcPriceHistoryAsync(1, "", 
                DateTime.Now, DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_btc_prices_history_if_dtos_is_null()
    {
        _cryptoInfoService.Setup(s => s.GetPriceHistoryAsync(It.IsAny<string>(), It.IsAny<DateTime>(), 
                It.IsAny<DateTime>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<ProfitToBtcHistoryDto>>(null));

        var result = _controller.GetBtcPriceHistoryAsync(1, "", 
                DateTime.Now, DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
}