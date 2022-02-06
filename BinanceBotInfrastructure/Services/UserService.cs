using System;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly IHttpClientService _httpClientService;
        private readonly IWebSocketClientService _wsClientService;
        public UserService(IBinanceBotDbContext db, IHttpClientService httpClientService, 
            IWebSocketClientService wsClientService)
        {
            _db = db;
            _httpClientService = httpClientService;
            _wsClientService = wsClientService;
        }

        public async Task<int> UpdateUserInfoAsync(UserBaseDto userDto,
            CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id, token);

            if (user is null || string.IsNullOrEmpty(userDto.Login))
                return 0;
            
            user.Login = userDto.Login;

            if (!string.IsNullOrEmpty(userDto.Name))
                user.Name = userDto.Name;
            
            if (!string.IsNullOrEmpty(userDto.Surname))
                user.Surname = userDto.Surname;
            
            if (!string.IsNullOrEmpty(userDto.Email))
                user.Email = userDto.Email;

            return await _db.SaveChangesAsync(token);
        }

        public async Task<int> SaveApiKeysAsync(ApiKeysDto apiKeysDto,
            CancellationToken token)
        {
            var userSettings = await _db.UserSettings.FirstOrDefaultAsync(s => s.IdUser == apiKeysDto.Id,
                    token);

            if (userSettings is null)
                return 0;

            userSettings.ApiKey = apiKeysDto.ApiKey;
            userSettings.SecretKey = apiKeysDto.SecretKey;

            return await _db.SaveChangesAsync(token);
        }

        public async Task GetUserDataStreamAsync(string listenKey, Action<string> handler,
            CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetUserDataStreamEndpoint(listenKey);
            
            await _wsClientService.ConnectToWebSocketAsync(uri, "", Console.WriteLine, token );
        }
    }
}