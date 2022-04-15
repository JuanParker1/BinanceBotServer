using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.Data;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using BinanceBotInfrastructure.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class RequestTrackerServiceTests
{
    private readonly CacheDb _cache;
    private readonly IBinanceBotDbContext _db;
    private readonly RequestTrackerService _service;

    public RequestTrackerServiceTests()
    {
        _cache = new CacheDb();
        
        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "RequestTrackerTests")
            .Options;
        _db = new BinanceBotDbContext(options);

        var requests = new List<Request>
        {
            new() {Id = 1, IdUser = 1, Date = new DateTime(2022, 03, 10)},
            new() {Id = 2, IdUser = 1, Date = new DateTime(2022, 04, 12)},
            new() {Id = 3, IdUser = 2, Date = new DateTime(2022, 04, 12)}
        };

        if (_db.RequestLog.Any())
        {
            _db.RequestLog.RemoveRange(_db.RequestLog.Where(b => b.Id > 0));
            _db.SaveChanges();
        }

        _db.RequestLog.AddRange(requests);
        _db.SaveChanges();

        _service = new RequestTrackerService(_db, _cache);
    }
    
    ~RequestTrackerServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_entities_in_get_user_requests()
    {
        var entites = await _service.GetUserRequestsAsync(1,
            100, CancellationToken.None);
        
        Assert.NotNull(entites);
    }
    
    [Fact]
    public async void It_should_return_two_entities_in_get_user_requests()
    {
        var entites = await _service.GetUserRequestsAsync(1,
            100, CancellationToken.None);
        
        Assert.Equal(2,entites.Count());
    }
    
    [Fact]
    public async void It_should_return_new_request_id_in_register_request()
    {
        var requestDto = new RequestDto {IdUser = 3};
        
        var result = await _service.RegisterRequestAsync(requestDto,
            CancellationToken.None);
        
        Assert.Equal(4, result);
    }
    
    [Fact]
    public async void It_should_return_false_if_request_not_new_register_request()
    {
        var requestDto = new RequestDto
        {
            IdUser = 1, 
            Date = new DateTime(2022, 04, 12)
        };
        
        var result = await _service.RegisterRequestAsync(requestDto,
            CancellationToken.None);
        
        Assert.Equal(-1, result);
    }
}