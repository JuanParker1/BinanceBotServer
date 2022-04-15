using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.Data;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace BinanceBotWebApi.Tests.ServicesTests;

public class CrudServiceTests
{
    private readonly IBinanceBotDbContext _db;
    private readonly CrudServiceMock _service;
    
    // CrudService has protected constructor. Example CrudServiceMock class in used as example
    // on BalanceChange Db table.
    private class CrudServiceMock : CrudService<BalanceChangeDto, BalanceChange>
    {
        public CrudServiceMock(IBinanceBotDbContext db) : base(db) { }
    }

    public CrudServiceTests()
    {
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

        _service = new CrudServiceMock(_db);
    }
    
    ~CrudServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_pagination_container_in_get_page()
    {
        var container = await _service.GetPageAsync();
        
        Assert.NotNull(container);
    }
    
    [Fact]
    public async void It_should_return_three_items_container_in_get_page()
    {
        var container = await _service.GetPageAsync();
        
        Assert.Equal(3, container.Items.Count);
    }
    
    [Fact]
    public async void It_should_return_correct_entity_by_id_in_get()
    {
        var entity = await _service.GetAsync(1, 
            CancellationToken.None);
        
        Assert.Equal(10, entity.Amount);
    }
    
    [Fact]
    public async void It_should_return_three_items_in_get_all()
    {
        var entities = await _service.GetAllAsync(CancellationToken.None);
        
        Assert.Equal(3, entities.Count());
    }

    [Fact]
    public async void It_should_return_correct_number_of_saved_items_in_insert_range()
    {
        var savedAmount = await _service.InsertRangeAsync(new List<BalanceChangeDto>
            {
                new(),
                new()
            }, 
            CancellationToken.None);
        
        Assert.Equal(2, savedAmount);
    }
    
    [Fact]
    public async void It_should_return_correct_number_of_updated_items_in_update()
    {
        var newDto = new BalanceChangeDto
        {
            Id = 2,
            Amount = 1000
        };
        
        _db.ChangeTracker.Clear();
        
        var updatedAmount = await _service.UpdateAsync(2, 
            newDto, CancellationToken.None);
        
        Assert.Equal(1, updatedAmount);
    }
    
    [Fact]
    public async void It_should_return_correct_number_of_items_in_get_existing_entities()
    {
        var entities = await _service.GetExistingEntitiesAsync(new List<int> {2,3}, 
            CancellationToken.None);
        
        Assert.Equal(2, entities.Count());
    }
    
    [Fact]
    public async void It_should_return_correct_number_of_deleted_items_in_delete()
    {
        _db.ChangeTracker.Clear();
        
        var deletedEntitiesAmount = await _service.DeleteAsync(2, 
            CancellationToken.None);
        
        Assert.Equal(1, deletedEntitiesAmount);
    }
    
    [Fact]
    public async void It_should_return_correct_number_of_deleted_items_in_delete_range()
    {
        _db.ChangeTracker.Clear();
        
        var deletedEntitiesAmount = await _service.DeleteRangeAsync(new List<int> {2,3}, 
            CancellationToken.None);
        
        Assert.Equal(2, deletedEntitiesAmount);
    }
}