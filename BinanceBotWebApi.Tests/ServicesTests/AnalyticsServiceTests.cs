using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.Data.Analytics;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class AnalyticsServiceTests
{
    private readonly Mock<ICryptoInfoService> _cryptoInfoService;
    private readonly IBinanceBotDbContext _db;
    private readonly AnalyticsService _service;
    
    public AnalyticsServiceTests()
    {
        _cryptoInfoService = new Mock<ICryptoInfoService>();

        _cryptoInfoService.Setup(s => s.GetPriceHistoryAsync(It.IsAny<string>(), 
                It.IsAny<DateTime>(),It.IsAny<DateTime>(), CancellationToken.None).Result)
            .Returns(new List<ProfitToBtcHistoryDto>
            {
                new() {Date = new DateTime(2022, 03, 21), BtcPrice = 43100},
                new() {Date = new DateTime(2022, 04, 10), BtcPrice = 43120},
                new() {Date = new DateTime(2022, 04, 12), BtcPrice = 43110}
            });
        
        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "AnalyticsTests")
            .Options;
        _db = new BinanceBotDbContext(options);

        var entities = new List<Order>
        {
            new() 
            {
                Id = 1, IdUser = 1, DateCreated = new DateTime(2022, 03, 21), IdSide = 1, IdType = 1, 
                IdCreationType = 1, Quantity = 0.51, Price = 20, DateClosed = new DateTime(2022, 03, 22),
                ClientOrderId = "F3Pfm55SpnkDSHMrLHBIat", OrderId = 2448037895, IdOrderStatus = 2
            },
            new() 
            {
                Id = 2, IdUser = 1, DateCreated = new DateTime(2022, 04, 10), IdSide = 1, IdType = 1, 
                IdCreationType = 1, Quantity = 0.001, Price = 43120, DateClosed = new DateTime(2022, 04, 10),
                ClientOrderId = null, OrderId = 0, IdOrderStatus = 2
            },
            new() 
            {
                Id = 3, IdUser = 2, DateCreated = new DateTime(2022, 04, 12), IdSide = 1, IdType = 1, 
                IdCreationType = 2, Quantity = 0.51, Price = 20, DateClosed = null,
                ClientOrderId = "web_d47234e250994cdcbf0cc0797ff81d45", OrderId = 2514486585, IdOrderStatus = 1
            },
            new() 
            {
                Id = 4, IdUser = 2, DateCreated = new DateTime(2022, 04, 12), IdSide = 1, IdType = 1, 
                IdCreationType = 2, Quantity = 0.034, Price = 300, DateClosed = null,
                ClientOrderId = "web_b03b69fca53d418fae421b4a7c6d94df", OrderId = 3876584022, IdOrderStatus = 1
            }
        };

        if (_db.Orders.Any())
        {
            _db.Orders.RemoveRange(entities);
            _db.SaveChanges();
        }

        _db.Orders.AddRange(entities);
        _db.SaveChanges();

        _service = new AnalyticsService(_db, _cryptoInfoService.Object);
    }

    ~AnalyticsServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_result_dto_in_get_profit_to_btc()
    {
        var result = await _service.GetProfitToBtcAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async void It_should_return_correct_average_order_lifetime_in_get_profit_to_btc()
    {
        var result = await _service.GetProfitToBtcAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(1440, result.AverageOrderLifeTimeMinutes);
    }
    
    [Fact]
    public async void It_should_return_correct_current_btc_price_in_get_profit_to_btc()
    {
        var result = await _service.GetProfitToBtcAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(43110, result.CurrentBtcPrice);
    }

    [Fact]
    public async void It_should_return_correct_total_orders_closed_in_get_profit_to_btc()
    {
        var result = await _service.GetProfitToBtcAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(2, result.TotalOrdersClosed);
    }
    
    [Fact]
    public async void It_should_return_correct_total_orders_cancelled_in_get_profit_to_btc()
    {
        var result = await _service.GetProfitToBtcAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(2, result.TotalOrdersCancelled);
    }
    
    [Fact]
    public async void It_should_return_correct_total_profit_in_get_profit_to_btc()
    {
        var result = await _service.GetProfitToBtcAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(-53, Math.Round(result.TotalProfit));
    }
    
    [Fact]
    public async void It_should_return_correct_profit_history_in_get_profit_to_btc()
    {
        var result = await _service.GetProfitToBtcAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(3, Math.Round((double)result.Data.Count()));
    }
    
    [Fact]
    public async void It_should_return_trade_types_stats_in_get_trade_types_stats()
    {
        var result = await _service.GetTradeTypesStatsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async void It_should_return_correct_signal_orders_rate_in_get_trade_types_stats()
    {
        var result = await _service.GetTradeTypesStatsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(0, result.SignalOrdersRate);
    }
    
    [Fact]
    public async void It_should_return_correct_stop_orders_rate_in_get_trade_types_stats()
    {
        var result = await _service.GetTradeTypesStatsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(100, result.StopOrdersRate);
    }
    
    [Fact]
    public async void It_should_return_correct_manual_orders_rate_in_get_trade_types_stats()
    {
        var result = await _service.GetTradeTypesStatsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(0, result.ManualOrdersRate);
    }
    
    [Fact]
    public async void It_should_return_correct_signal_orders_profit_in_get_trade_types_stats()
    {
        var result = await _service.GetTradeTypesStatsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(0, result.SignalOrdersProfit);
    }
    
    [Fact]
    public async void It_should_return_correct_stop_orders_profit_in_get_trade_types_stats()
    {
        var result = await _service.GetTradeTypesStatsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(-53, Math.Round((double)result.StopOrdersProfit));
    }
    
    [Fact]
    public async void It_should_return_correct_manual_orders_profit_in_get_trade_types_stats()
    {
        var result = await _service.GetTradeTypesStatsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(0, result.ManualOrdersProfit);
    }
    
    [Fact]
    public async void It_should_return_profit_details_in_get_profit_details()
    {
        var result = await _service.GetProfitDetailsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async void It_should_return_two_entities_in_get_profit_details()
    {
        var result = await _service.GetProfitDetailsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async void It_should_return_correct_signal_profit_in_get_profit_details()
    {
        var result = await _service.GetProfitDetailsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(0, result.First().SignalOrdersProfit);
    }
    
    [Fact]
    public async void It_should_return_correct_stop_profit_in_get_profit_details()
    {
        var result = await _service.GetProfitDetailsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(43, result.First().StopOrdersProfit);
    }
    
    [Fact]
    public async void It_should_return_correct_manual_profit_in_get_profit_details()
    {
        var result = await _service.GetProfitDetailsAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(0, result.First().ManualOrdersProfit);
    }
}