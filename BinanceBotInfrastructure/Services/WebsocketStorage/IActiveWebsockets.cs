using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace BinanceBotInfrastructure.Services.WebsocketStorage
{
    public interface IActiveWebsockets
    {
        (ClientWebSocket prices, ClientWebSocket userData) Get(int idUser);
        Task<bool> RemoveAsync(int idUser, CancellationToken token);
    }
}