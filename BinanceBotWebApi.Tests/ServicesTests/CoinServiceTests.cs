using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;
using BinanceBotApp.Services.BackgroundWorkers;
using BinanceBotInfrastructure.Services;
using BinanceBotInfrastructure.Services.CoinPricesStorage;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class CoinServiceTests
{
    private readonly Mock<ISettingsService> _settingsService;
    private readonly Mock<IHttpClientService> _httpService;
    private readonly Mock<IOrdersService> _ordersService;
    private readonly Mock<IEventsService> _eventsService;
    private readonly Mock<IWebSocketClientService> _websocketService;
    private readonly Mock<ICoinPricesStorage> _coinPricesStorage;
    private readonly Mock<IRefreshOrderBackgroundQueue> _ordersQueue;
    private readonly CoinService _service;

    public CoinServiceTests()
    {
        _settingsService = new Mock<ISettingsService>();
        _httpService = new Mock<IHttpClientService>();
        _ordersService = new Mock<IOrdersService>();
        _eventsService = new Mock<IEventsService>();
        _websocketService = new Mock<IWebSocketClientService>();
        _coinPricesStorage = new Mock<ICoinPricesStorage>();
        _ordersQueue = new Mock<IRefreshOrderBackgroundQueue>();
        
        _settingsService.Setup(s => s.GetApiKeysAsync(It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(("", ""));
        
        _httpService.Setup(s => s.ProcessRequestAsync<IEnumerable<CoinPrice>>(It.IsAny<Uri>(), 
                It.IsAny<IDictionary<string, string>>(), It.IsAny<(string, string)>(), 
                It.IsAny<HttpMethods>(), CancellationToken.None).Result)
            .Returns(new List<CoinPrice>
            {
                new () {Symbol = "BTCUSDT", Price = "2000"},
                new () {Symbol = "ETHUSDT", Price = "1000"}
            });

        _service = new CoinService(_settingsService.Object,
            _httpService.Object, _ordersService. Object,
            _eventsService.Object, _websocketService.Object,
            _coinPricesStorage.Object, _ordersQueue.Object);
    }
    
    [Fact]
    public async void It_should_return_entity_in_get_trading_pairs()
    {
        var result = await _service.GetTradingPairsAsync(1,
            CancellationToken.None);
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async void It_should_return_two_coin_infos_in_get_trading_pairs()
    {
        var result = await _service.GetTradingPairsAsync(1,
            CancellationToken.None);
        
        Assert.Equal(2, result.Count());
    }
}