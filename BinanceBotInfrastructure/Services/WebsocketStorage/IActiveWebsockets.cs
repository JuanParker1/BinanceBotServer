using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.DataInternal;

namespace BinanceBotInfrastructure.Services.WebsocketStorage
{
    public interface IActiveWebsockets
    {
        (WebSocketWrapper prices, WebSocketWrapper price, WebSocketWrapper userData) Get(int idUser);
        Task<bool> RemoveAsync(int idUser, CancellationToken token);
    }
}