using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class UserServiceTests
{
    private readonly Mock<IAuthService> _authService;
    private readonly Mock<IWebSocketClientService> _webSocketService;
    private readonly IBinanceBotDbContext _db;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _authService = new Mock<IAuthService>();
        _webSocketService = new Mock<IWebSocketClientService>();
        
        _authService.Setup(s => s.ChangePasswordAsync(It.IsAny<ChangePasswordDto>(), 
                CancellationToken.None).Result)
            .Returns(1);
        
        var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
            .UseInMemoryDatabase(databaseName: "BinanceBotTests")
            .Options;
        _db = new BinanceBotDbContext(options);

        var users = new List<User> { new() {Id = 1, Login = "User"} };
        
        if (_db.Users.Any())
        {
            _db.Users.RemoveRange(_db.Users.Where(u => u.Id > 0));
            _db.SaveChanges();
        }

        _db.Users.AddRange(users);
        _db.SaveChanges();
        
        _service = new UserService(_db, _authService.Object, 
            _webSocketService.Object);
    }
    
    ~UserServiceTests()
    {
        _db.Dispose();
    }
    
    [Fact]
    public async void It_should_return_user_info_in_get_user_info()
    {
        var user = await _service.GetUserInfoAsync(1,
            CancellationToken.None);
        
        Assert.NotNull(user);
    }
    
    [Fact]
    public async void It_should_change_user_login_in_update_user_info()
    {
        await _service.UpdateUserInfoAsync(new UserInfoDto {Id = 1, Login = "Another", Name = "", Surname = "", Email = ""},
            CancellationToken.None);
        
        var user = await _service.GetUserInfoAsync(1,
            CancellationToken.None);
        
        Assert.Equal("Another", user.Login);
    }
    
    [Fact]
    public async void It_should_return_correct_result_in_change_password()
    {
        var result = await _service.ChangePasswordAsync(new ChangePasswordDto(),
            CancellationToken.None);

        Assert.Equal(1, result);
    }
}