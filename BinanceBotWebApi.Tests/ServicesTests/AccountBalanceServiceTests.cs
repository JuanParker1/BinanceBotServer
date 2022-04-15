using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class AccountBalanceServiceTests
{
    private readonly Mock<ISettingsService> _settingsService;
    private readonly Mock<IHttpClientService> _httpService;
    private readonly IBinanceBotDbContext _db;
    private readonly AccountBalanceService _service;

    public AccountBalanceServiceTests()
    {
        _settingsService = new Mock<ISettingsService>();
        _httpService = new Mock<IHttpClientService>();
        
        _settingsService.Setup(s => s.GetApiKeysAsync(It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(("", ""));
        
        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "BinanceBotTests")
            .Options;
        _db = new BinanceBotDbContext(options);

        var balanceChanges = new List<BalanceChange>
        {
            new() {Id = 1, IdUser = 1, Date = new DateTime(2022, 03, 10), IdDirection = 1, Amount = 10},
            new() {Id = 2, IdUser = 1, Date = new DateTime(2022, 04, 12), IdDirection = 2, Amount = 1},
            new() {Id = 3, IdUser = 2, Date = new DateTime(2022, 04, 12), IdDirection = 2, Amount = 100}
        };

        if (_db.BalanceChanges.Any())
        {
            _db.BalanceChanges.RemoveRange(_db.BalanceChanges.Where(b => b.Id > 0));
            _db.SaveChanges();
        }

        _db.BalanceChanges.AddRange(balanceChanges);
        _db.SaveChanges();

        _service = new AccountBalanceService(_db, _settingsService.Object, 
            _httpService.Object);
    }

    ~AccountBalanceServiceTests()
    {
        _db.Dispose();
    }

    [Fact]
    public async void It_should_return_two_entities_in_get_all()
    {
        var entites = await _service.GetAllAsync(1,
            DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(2,entites.Count());
    }
    
    [Fact]
    public async void It_should_return_one_entity_for_time_interval_in_get_all()
    {
        var entites = await _service.GetAllAsync(1,
            new DateTime(2022, 04, 01), new DateTime(2022, 04, 30), 
            CancellationToken.None);
        
        Assert.Single(entites);
    }
    
    [Fact]
    public async void It_should_return_two_entities_in_get_current_balance()
    {
        _httpService.Setup(s => s.ProcessRequestAsync<AccountBalanceInfo>(It.IsAny<Uri>(), 
                It.IsAny<IDictionary<string, string>>(), It.IsAny<(string, string)>(), 
                It.IsAny<HttpMethods>(), CancellationToken.None).Result)
            .Returns(new AccountBalanceInfo
            {
                Balances = new List<CoinAmountDto>
                {
                    new () { Free = 10, Locked = 11 },
                    new () { Free = 100, Locked = 22 }
                }
            });

        
        var entites = await _service.GetCurrentBalanceAsync(1, 
            CancellationToken.None);
        
        Assert.Equal(2,entites.Count());
    }
    
    [Fact]
    public async void It_should_return_correct_total_balance_changes_in_get_total_balance()
    {
        var entites = await _service.GetTotalBalanceAsync(1, 
            CancellationToken.None);
        
        Assert.Equal(10, entites.TotalDeposit);
        Assert.Equal(1, entites.TotalWithdraw);
    }
}