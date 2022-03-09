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
using BinanceBotInfrastructure.Services.WebsocketStorage;
using Mapster;

namespace BinanceBotInfrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly IAuthService _authService;
        private readonly IWebSocketClientService _wsService;
        private readonly IActiveWebsockets _activeWebsockets;
  
        public UserService(IBinanceBotDbContext db, IAuthService authService,
            IWebSocketClientService wsService, IActiveWebsockets activeWebsockets)
        {
            _db = db;
            _authService = authService;
            _wsService = wsService;
            _activeWebsockets = activeWebsockets;
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
            Action<string> handler, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetUserDataStreamEndpoint(listenKey);
            
            await _wsService.ConnectToWebSocketAsync(uri, "", idUser,
                WebsocketConnectionTypes.UserData, handler, token);
        }
        
        public async Task GetSubscriptionsListAsync(int idUser, CancellationToken token)
        {
            var wsClient = _activeWebsockets.Get(idUser).userData; //TODO: в .prices надо кидать, а не в userdata. Там же ничего нет.
            var data = $"{{\"method\": \"LIST_SUBSCRIPTIONS\",\"id\": 1}}"; // Надо закидывать запрос в тот же открытый инстанс WS клиента. Как и отписываться от стрима монет.
            
            await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.UserData, Console.WriteLine, token ); // TODO: Handler надо принять из контроллера
        }
    }
}