using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Mapster;

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

        public async Task<AuthUserInfoDto> GetUserInfoAsync(int idUser,
            CancellationToken token = default)
        {
            var authUserInfo = await _db.Users.FirstOrDefaultAsync(u => u.Id == idUser, 
                token);

            var authUserInfoDto = authUserInfo?.Adapt<AuthUserInfoDto>();
            
            return authUserInfoDto;
        }

        public async Task<int> UpdateUserInfoAsync(AuthUserInfoDto authUserDto,
            CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => 
                u.Id == authUserDto.Id, token);

            if (user is null || string.IsNullOrEmpty(authUserDto.Login.Trim()))
                return 0;
            
            user.Login = authUserDto.Login.Trim();

            if (!string.IsNullOrEmpty(authUserDto.Name.Trim()))
                user.Name = authUserDto.Name.Trim();
            
            if (!string.IsNullOrEmpty(authUserDto.Surname.Trim()))
                user.Surname = authUserDto.Surname.Trim();
            
            if (!string.IsNullOrEmpty(authUserDto.Email.Trim()))
                user.Email = authUserDto.Email.Trim();

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