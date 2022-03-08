using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BinanceBotWebApi.SignalR
{
    // SignalR manual:
    // https://docs.microsoft.com/ru-ru/aspnet/core/signalr/introduction?view=aspnetcore-5.0

    [Authorize]
    public class PricesHub : Hub
    {
        public Task AddToGroup(string groupName)
            => Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        
        public Task RemoveFromGroup(string groupName)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}