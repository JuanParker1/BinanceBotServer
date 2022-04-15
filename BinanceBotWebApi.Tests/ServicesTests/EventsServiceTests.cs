using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class EventsServiceTests
{
    private readonly IBinanceBotDbContext _db;
    private readonly EventsService _service;
    
    public EventsServiceTests()
    {
        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "BinanceBotTests")
            .Options;
        _db = new BinanceBotDbContext(options);

        if (_db.EventLog.Any())
        {
            _db.EventLog.RemoveRange(_db.EventLog.Where(e => e.Id > 0));
            _db.SaveChanges();
        }
        
        if (_db.EventTemplates.Any())
        {
            _db.EventTemplates.RemoveRange(_db.EventTemplates.Where(e => e.Id > 0));
            _db.SaveChanges();
        }

        _db.EventLog.AddRange(new List<Event>
        {
            new() {Id = 1, IdUser = 1, Date = new DateTime(2022, 03, 10), IsRead = true},
            new() {Id = 2, IdUser = 1, Date = new DateTime(2022, 03, 12), IsRead = true},
            new() {Id = 3, IdUser = 1, Date = new DateTime(2022, 04, 01), IsRead = false},
            new() {Id = 4, IdUser = 1, Date = new DateTime(2022, 04, 03), IsRead = false},
            new() {Id = 5, IdUser = 1, Date = new DateTime(2022, 04, 10), IsRead = false}
        });
        _db.EventTemplates.AddRange(new List<EventTemplate>
        {
            new() {Id = 1, Template = "Some {0} template"}
        });
        _db.SaveChanges();

        _service = new EventsService(_db);
    }

    ~EventsServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_five_entities_in_get_all()
    {
        var entites = await _service.GetAllAsync(1,
            false,DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(5,entites.Count());
    }
    
    [Fact]
    public async void It_should_return_three_unread_entities_in_get_all()
    {
        var entites = await _service.GetAllAsync(1,
            true,DateTime.MinValue, DateTime.MaxValue, 
            CancellationToken.None);
        
        Assert.Equal(3,entites.Count());
    }
    
    [Fact]
    public async void It_should_return_two_entities_for_march_in_get_all()
    {
        var entites = await _service.GetAllAsync(1,
            false,new DateTime(2022, 03, 01), 
            new DateTime(2022, 03, 30), 
            CancellationToken.None);
        
        Assert.Equal(2,entites.Count());
    }
    
    [Fact]
    public async void It_should_return_correct_string_in_create_event_text()
    {
        var entity = await _service.CreateEventTextAsync(EventTypes.OrderCreated,
            new List<string>  {"good"}, CancellationToken.None);
        
        Assert.Equal("Some good template", entity);
    }
    
    [Fact]
    public async void It_should_create_event_with_correct_id()
    {
        var newEntityId = await _service.CreateEventAsync(1, "some event text", 
            CancellationToken.None);

        Assert.Equal(6, newEntityId);
    }
}