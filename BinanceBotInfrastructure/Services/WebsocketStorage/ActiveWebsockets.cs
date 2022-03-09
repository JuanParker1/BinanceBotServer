using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace BinanceBotInfrastructure.Services.WebsocketStorage
{
    public class ActiveWebsockets : IActiveWebsockets
    {
        private readonly ConcurrentDictionary<int, (ClientWebSocket prices, ClientWebSocket userData)> _activeWebsocketsStorage;

        public ActiveWebsockets()
        {
            _activeWebsocketsStorage = new ConcurrentDictionary<int, (ClientWebSocket prices, ClientWebSocket userData)>();
        }

        public (ClientWebSocket prices, ClientWebSocket userData) Get(int idUser)
        {
            var pricesStream = new ClientWebSocket();
            var userDataStream = new ClientWebSocket();
            return _activeWebsocketsStorage.GetOrAdd(idUser, (pricesStream, userDataStream));
        }

        public async Task<bool> RemoveAsync(int idUser, CancellationToken token)
        {
            var (prices, userData) = Get(idUser);
            await prices.CloseAsync(WebSocketCloseStatus.Empty, "", token);
            prices.Dispose();
            await userData.CloseAsync(WebSocketCloseStatus.Empty, "", token);
            userData.Dispose();
            return _activeWebsocketsStorage.TryRemove(idUser, out var result);
        }
    }
}