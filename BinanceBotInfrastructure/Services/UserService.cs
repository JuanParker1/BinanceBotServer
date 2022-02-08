using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.Services;
using BinanceBotDb.Models;

namespace BinanceBotInfrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly IHttpClientService _httpService;
        private readonly IWebSocketClientService _wsService;
        public UserService(IBinanceBotDbContext db, IHttpClientService httpService, 
            IWebSocketClientService wsService)
        {
            _db = db;
            _httpService = httpService;
            _wsService = wsService;
        }

        public async Task<int> UpdateUserInfoAsync(UserBaseDto userDto,
            CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => 
                u.Id == userDto.Id, token);

            if (user is null || string.IsNullOrEmpty(userDto.Login.Trim()))
                return 0;
            
            user.Login = userDto.Login.Trim();

            if (!string.IsNullOrEmpty(userDto.Name.Trim()))
                user.Name = userDto.Name.Trim();
            
            if (!string.IsNullOrEmpty(userDto.Surname.Trim()))
                user.Surname = userDto.Surname.Trim();
            
            if (!string.IsNullOrEmpty(userDto.Email.Trim()))
                user.Email = userDto.Email.Trim();

            return await _db.SaveChangesAsync(token);
        }

        public async Task GetUserDataStreamAsync(string listenKey, 
            Action<string> handler, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetUserDataStreamEndpoint(listenKey);
            
            await _wsService.ConnectToWebSocketAsync(uri, "", 
                Console.WriteLine, token );
        }
    }
}