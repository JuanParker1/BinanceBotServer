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

public class CoinControllerTests
{
    private readonly Mock<ICoinService> _coinService;
    private readonly Mock<IHubContext<PricesHub>> _pricesHubContext;
    private readonly CoinController _controller;

    public CoinControllerTests()
    {
        _coinService = new Mock<ICoinService>();
        _pricesHubContext = new Mock<IHubContext<PricesHub>>();
        
        _controller = new CoinController(_coinService.Object, 
            _pricesHubContext.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_trading_pairs_if_wrong_user_id()
    {
        var result = _controller.GetTradingPairsAsync(0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_in_get_trading_pairs_if_dtos_is_not_null()
    {
        _coinService.Setup(s => s.GetTradingPairsAsync(It.IsAny<int>(),
                CancellationToken.None).Result)
            .Returns(new List<string>());

        var result = _controller.GetTradingPairsAsync(1).Result;
        var okResult = result as OkObjectResult;
    
        Assert.NotNull(okResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_get_trading_pairs_if_dtos_is_null()
    {
        _coinService.Setup(s => s.GetTradingPairsAsync(It.IsAny<int>(), 
                CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<string>>(null));

        var result = _controller.GetTradingPairsAsync(1).Result;
        var okResult = result as OkObjectResult;
    
        Assert.Null(okResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_single_coin_price_stream_if_wrong_user_id()
    {
        var result = _controller.GetSingleCoinPriceStreamAsync("", 0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_in_get_single_coin_price_stream_if_right_user_id()
    {
        var result = _controller.GetSingleCoinPriceStreamAsync("", 1)
            .Result;
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
    
    [Fact]
    public void It_should_return_403_in_get_coin_prices_stream_if_wrong_user_id()
    {
        var result = _controller.GetCoinsPricesStreamAsync(new GenericCollectionDto<string>(), 0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_in_get_coin_prices_stream_if_right_user_id()
    {
        var result = _controller.GetCoinsPricesStreamAsync(new GenericCollectionDto<string>(), 1)
            .Result;
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
    
    [Fact]
    public void It_should_return_403_in_unsubscribe_coin_price_stream_if_wrong_user_id()
    {
        var result = _controller.UnsubscribeSingleCoinPriceStreamAsync("", 0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_in_unsubscribe_coin_price_stream_if_right_user_id()
    {
        var result = _controller.UnsubscribeSingleCoinPriceStreamAsync("", 1)
            .Result;
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
    
    [Fact]
    public void It_should_return_403_in_unsubscribe_coin_prices_stream_if_wrong_user_id()
    {
        var result = _controller.UnsubscribeCoinPricesStreamAsync(new GenericCollectionDto<string>(), 0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_in_unsubscribe_coin_prices_stream_if_right_user_id()
    {
        var result = _controller.UnsubscribeCoinPricesStreamAsync(new GenericCollectionDto<string>(), 1)
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
    public void It_should_return_ok_in_get_subscriptions_list_if_right_user_id()
    {
        var result = _controller.GetSubscriptionsListAsync(1)
            .Result;
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
}