using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.Data;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using BinanceBotInfrastructure.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class AuthServiceTests
{
    private readonly Mock<ICacheDb> _cacheDb;
    private readonly IBinanceBotDbContext _db;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _cacheDb = new Mock<ICacheDb>();
        
        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "AccountBalanceTests")
            .Options;
        _db = new BinanceBotDbContext(options);

        var users = new List<User>
        {
            new() {Id = 1, Login = "FirstUser", Password = "VzwA|6a4e3df1193666839c57ac8dcafe549cfb00fab0fdd78a008261332ba5c1a326ab93b6993a913219c2f8e078103b8f91",
                DateCreated = new DateTime(2022, 03, 10), Name = "Name", Surname = "Surname", Email = "Email"},
            new() {Id = 2, Login = "SecondUser", Password = "VzwA|6a4e3df1193666839c57ac8dcafe549cfb00fab0fdd78a008261332ba5c1a326ab93b6993a913219c2f8e078103b8f91",
                DateCreated = new DateTime(2022, 04, 12), Name = "Name", Surname = "Surname", Email = "Email"}
        };

        if (_db.BalanceChanges.Any())
        {
            _db.Users.RemoveRange(_db.Users.Where(b => b.Id > 0));
            _db.SaveChanges();
        }

        _db.Users.AddRange(users);
        _db.SaveChanges();

        _service = new AuthService(_db, _cacheDb.Object);
    }
    
    ~AuthServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_user_token_in_login()
    {
        var token = await _service.LoginAsync("FirstUser", "dev",
            CancellationToken.None);
        
        Assert.NotNull(token);
    }
    
    [Fact]
    public async void It_should_return_false_if_user_already_registered_in_register()
    {
        var result = await _service.RegisterAsync(new RegisterDto {Login = "FirstUser"},
            CancellationToken.None);
        
        Assert.False(result);
    }
    
    [Fact]
    public async void It_should_return_correct_value_if_no_such_user_in_change_password()
    {
        var dto = new ChangePasswordDto {IdUser = 5};
        
        var result = await _service.ChangePasswordAsync(dto,
            CancellationToken.None);
        
        Assert.Equal(-1, result);
    }
    
    [Fact]
    public async void It_should_return_correct_value_if_old_password_wrong_in_change_password()
    {
        var dto = new ChangePasswordDto {IdUser = 1, NewPassword = "qwerty", OldPassword = "wrong"};
        
        var result = await _service.ChangePasswordAsync(dto,
            CancellationToken.None);
        
        Assert.Equal(-2, result);
    }
    
    [Fact]
    public async void It_should_return_correct_value_if_password_changed_in_change_password()
    {
        var dto = new ChangePasswordDto {IdUser = 1, NewPassword = "qwerty", OldPassword = "dev"};
        
        var result = await _service.ChangePasswordAsync(dto,
            CancellationToken.None);
        
        Assert.Equal(1, result);
    }
}