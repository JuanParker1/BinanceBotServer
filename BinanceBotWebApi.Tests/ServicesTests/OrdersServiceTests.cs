using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;
using BinanceBotApp.Services.BackgroundWorkers;
using BinanceBotDb.Models;
using BinanceBotDb.Models.Directories;
using BinanceBotInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class OrdersServiceTests
{
    private readonly Mock<ISettingsService> _settingsService;
    private readonly Mock<IHttpClientService> _httpService;
    private readonly Mock<IEventsService> _eventsService;
    private readonly Mock<IAccountBalanceService> _accountBalanceService;
    private readonly Mock<IRefreshOrderBackgroundQueue> _ordersQueue;
    private readonly Mock<IConfiguration> _configuration;
    private readonly IBinanceBotDbContext _db;
    private readonly OrdersService _service;

    public OrdersServiceTests()
    {
        _settingsService = new Mock<ISettingsService>();
        _httpService = new Mock<IHttpClientService>();
        _eventsService = new Mock<IEventsService>();
        _accountBalanceService = new Mock<IAccountBalanceService>();
        _ordersQueue = new Mock<IRefreshOrderBackgroundQueue>();
        _configuration = new Mock<IConfiguration>();
        
        _settingsService.Setup(s => s.GetApiKeysAsync(It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(("", ""));
        
        _httpService.Setup(s => s.ProcessRequestAsync<OrderInfo>(It.IsAny<Uri>(), 
                It.IsAny<IDictionary<string, string>>(), It.IsAny<(string, string)>(), 
                It.IsAny<HttpMethods>(), CancellationToken.None).Result)
            .Returns(new OrderInfo {Symbol = "BTC", Price = "1000"});
        
        _httpService.Setup(s => s.ProcessRequestAsync<DeletedOrder>(It.IsAny<Uri>(), 
                It.IsAny<IDictionary<string, string>>(), It.IsAny<(string, string)>(), 
                It.IsAny<HttpMethods>(), CancellationToken.None).Result)
            .Returns(new DeletedOrder() {Symbol = "BTC", Price = "1000"});
        
        _httpService.Setup(s => s.ProcessRequestAsync<IEnumerable<DeletedOrder>>(It.IsAny<Uri>(), 
                It.IsAny<IDictionary<string, string>>(), It.IsAny<(string, string)>(), 
                It.IsAny<HttpMethods>(), CancellationToken.None).Result)
            .Returns(new List<DeletedOrder>
            {
                new() {Symbol = "BTC", Price = "1000"},
                new() {Symbol = "BTC", Price = "2000"},
            });

        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "OrdersTests")
            .Options;
        _db = new BinanceBotDbContext(options);
        
        if (_db.Orders.Any())
        {
            _db.Orders.RemoveRange(_db.Orders.Where(o => o.Id > 0));
            _db.SaveChanges();
        }

        _db.Orders.AddRange(new List<Order>
        {
            new() {Id = 1, IdUser = 1, Symbol = "ETH", DateCreated = new DateTime(2022, 03, 01), OrderId = 111, Price = 1000, OrderType = new OrderType {Caption = "c"}},
            new() {Id = 2, IdUser = 1, Symbol = "BTC", DateCreated = new DateTime(2022, 04, 01), OrderId = 222, Price = 2000, OrderType = new OrderType {Caption = "c"}},
            new() {Id = 3, IdUser = 1, Symbol = "BNB", DateCreated = new DateTime(2022, 04, 01), OrderId = 222, Price = 2000, OrderType = new OrderType {Caption = "c"}}
        });
        _db.SaveChanges();

        _service = new OrdersService(_db, _settingsService.Object, _httpService.Object,
            _eventsService.Object, _accountBalanceService.Object, _ordersQueue.Object,
            _configuration.Object);
    }
    
    ~OrdersServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_one_entity_in_get_order()
    {
        var orderInfo = await _service.GetOrderAsync(1,
            1, "", 1, CancellationToken.None);
        
        Assert.NotNull(orderInfo);
    }
    
    [Fact]
    public async void It_should_return_one_order_with_correct_coinname_in_get_order()
    {
        var orderInfo = await _service.GetOrderAsync(1,
            1, "", 1, CancellationToken.None);
        
        Assert.Equal("BTC", orderInfo.Symbol);
    }
    
    [Fact]
    public async void It_should_return_two_orders_in_get_orders_for_pair()
    {
        _httpService.Setup(s => s.ProcessRequestAsync<IEnumerable<OrderInfo>>(It.IsAny<Uri>(), 
                It.IsAny<IDictionary<string, string>>(), It.IsAny<(string, string)>(), 
                It.IsAny<HttpMethods>(), CancellationToken.None).Result)
            .Returns(new List<OrderInfo> 
            {
                new() {Symbol = "BTC", Price = "1000"},
                new() {Symbol = "BTC", Price = "2000"}
            });
        
        var orderInfos = await _service.GetOrdersForPairAsync(1,
            "", 1, CancellationToken.None);
        
        Assert.Equal(2, orderInfos.Count());
    }
    
    [Fact]
    public async void It_should_return_three_active_orders_in_get_active_orders()
    {
        _httpService.Setup(s => s.ProcessRequestAsync<IEnumerable<OrderInfo>>(It.IsAny<Uri>(), 
                It.IsAny<IDictionary<string, string>>(), It.IsAny<(string, string)>(), 
                It.IsAny<HttpMethods>(), CancellationToken.None).Result)
            .Returns(new List<OrderInfo> 
            {
                new() {Symbol = "ETH", OrderId = 111, Price = "1000", Side = "BUY", Type = "LIMIT", OrigQty = "5"},
                new() {Symbol = "BTC", OrderId = 222, Price = "2000", Side = "BUY", Type = "LIMIT", OrigQty = "5"},
                new() {Symbol = "BNB", OrderId = 222, Price = "2000", Side = "BUY", Type = "LIMIT", OrigQty = "5"}
            });
        
        _db.ChangeTracker.Clear();
        
        var orderInfos = await _service.GetActiveOrdersAsync(1, 
            1, CancellationToken.None);
        
        Assert.Equal(3, orderInfos.Count());
    }
    
    [Fact]
    public async void It_should_return_one_order_in_get_orders_history_for_pair()
    {
        var orderInfo = await _service.GetOrdersHistoryForPairAsync(1, 
            "BTC", 100000, CancellationToken.None);
        
        Assert.Single(orderInfo);
    }
    
    [Fact]
    public async void It_should_return_two_orders_in_get_orders_history_in_time_interval()
    {
        var orderInfos = await _service.GetOrdersHistoryAsync(1, 
            new DateTime(2022, 04, 01), new DateTime(2022, 04, 30), 
            CancellationToken.None);
        
        Assert.Equal(2, orderInfos.Count());
    }
    
    [Fact]
    public async void It_should_return_order_info_in_delete_order()
    {
        var orderInfo = await _service.DeleteOrderAsync(1, 
            1000000, "", "", 1, CancellationToken.None);
        
        Assert.NotNull(orderInfo);
    }
    
    [Fact]
    public async void It_should_return_two_order_infos_in_delete_all_orders_for_pair()
    {
        var orderInfo = await _service.DeleteAllOrdersForPairAsync(1, 
            "", 1000000, CancellationToken.None);
        
        Assert.NotNull(orderInfo);
    }
}