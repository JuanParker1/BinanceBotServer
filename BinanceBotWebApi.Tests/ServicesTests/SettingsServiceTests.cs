using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotDb.Models.Directories;
using BinanceBotInfrastructure.Services;
using BinanceBotInfrastructure.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class SettingsServiceTests
{
    private readonly Mock<IHttpClientService> _httpService;
    private readonly CacheDb _cache;
    private readonly IBinanceBotDbContext _db;
    private readonly SettingsService _service;

    public SettingsServiceTests()
    {
        _httpService = new Mock<IHttpClientService>();
        _cache = new CacheDb();
        
        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "RequestTrackerTests")
            .Options;
        _db = new BinanceBotDbContext(options);

        var settings = new List<Settings>
        {
            new() {Id = 1, IdUser = 1, ApiKey = "api", SecretKey = "secret", TradeMode = new TradeMode()},
            new() {Id = 2, IdUser = 1, TradeMode = new TradeMode()},
            new() {Id = 3, IdUser = 2, TradeMode = new TradeMode()}
        };

        if (_db.UserSettings.Any())
        {
            _db.UserSettings.RemoveRange(_db.UserSettings.Where(b => b.Id > 0));
            _db.SaveChanges();
        }

        _db.UserSettings.AddRange(settings);
        _db.SaveChanges();

        _service = new SettingsService(_db, _cache,
            _httpService.Object);
    }
    
    ~SettingsServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_settings_in_get_settings()
    {
        var entites = await _service.GetSettingsAsync(1,
            CancellationToken.None);
        
        Assert.NotNull(entites);
    }
    
    [Fact]
    public async void It_should_return_null_if_wrong_user_in_get_settings()
    {
        var entites = await _service.GetSettingsAsync(5,
            CancellationToken.None);
        
        Assert.Null(entites);
    }
}