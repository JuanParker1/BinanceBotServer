using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Services;

namespace BinanceBotWebApi.SignalR
{
    [Authorize]
    public class ConnectionStatusHub : Hub
    {
        private readonly IWebSocketClientService _webSocketService;
        private readonly ICoinService _coinService;

        public ConnectionStatusHub(IWebSocketClientService webSocketService,
            ICoinService coinService)
        {
            _webSocketService = webSocketService;
            _coinService = coinService;
        }
        public Task AddToGroup(string groupName) => // TODO: Async naming
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        public Task RemoveFromGroup(string groupName) => 
            Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        public async Task GetStatusAsync(int idUser)
        {
            var (prices, userData) = _webSocketService.GetConnections(idUser);
            await _coinService.GetSubscriptionsListAsync(idUser, CancellationToken.None);
            
            var dto = new ConnectionStatusDto
            {
                IsPricesStreamConnected = _webSocketService.IsAlive(prices.WebSocket),
                IsUserDataStreamConnected = _webSocketService.IsAlive(userData.WebSocket)
            };
            
            await Clients.Group($"User_{idUser}_Connection").SendAsync(
                nameof(IConnectionStatusHub.GetStatusAsync),
                dto,
                CancellationToken.None
            );
        }
    }
}