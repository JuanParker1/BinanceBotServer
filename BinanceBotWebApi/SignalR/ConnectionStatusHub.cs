using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BinanceBotApp.Data;

namespace BinanceBotWebApi.SignalR
{
    [Authorize]
    public class ConnectionStatusHub : Hub
    {
        public Task AddToGroup(string groupName) =>
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        public Task RemoveFromGroup(string groupName) => 
            Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        public Task GetStatus(int idUser)
        {
            var dto = new ConnectionStatusDto
            {
                IsPricesStreamConnected = true,
                IsUserDataStreamConnected = true,
                SubscriptionsList = new List<string> {"ethusdt@bookTicker", "bnbusdt@bookTicker", "tusdusdt@bookTicker"}
            };
            
            return Clients.Group($"User_{idUser}_Connection").SendAsync(
                nameof(IConnectionStatusHub.GetStatus),
                dto,
                CancellationToken.None
            );
        }
    }
}