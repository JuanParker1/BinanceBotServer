using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using BinanceBotWebApi.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class AccountBalanceControllerTests
{
    private readonly Mock<IAccountBalanceService> _balanceService;
    private readonly Mock<ICoinService> _coinService;
    private readonly Mock<IHubContext<PricesHub>> _pricesHubContext;
    private readonly AccountBalanceController _controller;

    public AccountBalanceControllerTests()
    {
        _balanceService = new Mock<IAccountBalanceService>();
        _coinService = new Mock<ICoinService>();
        _pricesHubContext = new Mock<IHubContext<PricesHub>>();
        
        _controller = new AccountBalanceController(_balanceService.Object,
            _coinService.Object, _pricesHubContext.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_deposit_history_if_wrong_user_id()
    {
        var result = _controller.GetDepositHistoryAsync(0, DateTime.Now, DateTime.Now)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_deposit_history_if_dtos_is_not_null()
    {
        _balanceService.Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<DateTime>(), CancellationToken.None).Result)
            .Returns(new List<BalanceChangeDto>());

        var result = _controller.GetDepositHistoryAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_deposit_history_if_dtos_is_null()
    {
        _balanceService.Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<DateTime>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<BalanceChangeDto>>(null));

        var result = _controller.GetDepositHistoryAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_current_balance_if_wrong_user_id()
    {
        var result = _controller.GetCurrentBalanceAsync(0)
            .Result;
        var forbidResult = result as ForbidResult;

        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_current_balance_if_dtos_is_not_null()
    {
        _balanceService.Setup(s => s.GetCurrentBalanceAsync(It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(new List<CoinAmountDto>());

        var result = _controller.GetCurrentBalanceAsync(1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_current_balance_if_dtos_is_null()
    {
        _balanceService.Setup(s => s.GetCurrentBalanceAsync(It.IsAny<int>(),
                CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<CoinAmountDto>>(null));

        var result = _controller.GetCurrentBalanceAsync(1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_total_balance_if_wrong_user_id()
    {
        var result = _controller.GetTotalBalanceAsync(0)
            .Result;
        var forbidResult = result as ForbidResult;

        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_total_balance_if_dtos_is_not_null()
    {
        _balanceService.Setup(s => s.GetTotalBalanceAsync(It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(new BalanceSummaryDto());

        var result = _controller.GetTotalBalanceAsync(1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_total_balance_if_dtos_is_null()
    {
        _balanceService.Setup(s => s.GetTotalBalanceAsync(It.IsAny<int>(),
                CancellationToken.None))
            .Returns(Task.FromResult<BalanceSummaryDto>(null));

        var result = _controller.GetTotalBalanceAsync(1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
}