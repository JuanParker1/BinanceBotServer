using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
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
        private readonly IAuthService _authService;
        public UserService(IBinanceBotDbContext db, IHttpClientService httpService, 
            IWebSocketClientService wsService, IAuthService authService)
        {
            _db = db;
            _httpService = httpService;
            _wsService = wsService;
            _authService = authService;
        }

        public async Task<UserInfoDto> GetUserInfoAsync(int idUser,
            CancellationToken token = default)
        {
            var user = await (from u in _db.Users
                                where u.Id == idUser
                                select u)
                            .FirstOrDefaultAsync(token);

            var authUserInfoDto = user?.Adapt<UserInfoDto>();
            
            return authUserInfoDto;
        }

        public async Task<int> UpdateUserInfoAsync(UserInfoDto userDto,
            CancellationToken token)
        {
            var user = await (from u in _db.Users
                                where u.Id == userDto.Id
                                select u)
                            .FirstOrDefaultAsync(token);

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

        public async Task<int> ChangePasswordAsync(ChangePasswordDto changePasswordDto,
            CancellationToken token)
        {
            var result = await _authService.ChangePasswordAsync(changePasswordDto, 
                token);

            return result;
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