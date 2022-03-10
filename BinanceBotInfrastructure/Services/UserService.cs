using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotApp.DataInternal.Enums;
using Mapster;

namespace BinanceBotInfrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly IAuthService _authService;
        private readonly IWebSocketClientService _webSocketService;

        public UserService(IBinanceBotDbContext db, IAuthService authService,
            IWebSocketClientService webSocketService)
        {
            _db = db;
            _authService = authService;
            _webSocketService = webSocketService;
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

        public async Task GetUserDataStreamAsync(string listenKey, int idUser,
            Action<string> responseHandler, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetUserDataStreamEndpoint(listenKey);
            
            var wsClientInstance = await _webSocketService.SendAsync(uri, "", idUser,
                WebsocketConnectionTypes.UserData, token);
            
            await _webSocketService.ListenAsync(wsClientInstance, responseHandler, token);
        }
        
        public async Task GetSubscriptionsListAsync(int idUser, Action<string> responseHandler, 
            CancellationToken token)
        { //TODO: в .prices надо кидать, а не в userdata. Там же ничего нет.
            var data = $"{{\"method\": \"LIST_SUBSCRIPTIONS\",\"id\": 1}}"; //TODO: Надо закидывать запрос в тот же открытый инстанс WS клиента. Как и отписываться от стрима монет.
            
            var wsClientInstance = await _webSocketService.SendAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.UserData, token );
            
            await _webSocketService.ListenAsync(wsClientInstance, responseHandler, token);
        }
    }
}