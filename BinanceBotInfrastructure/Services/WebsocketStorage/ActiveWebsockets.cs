using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using BinanceBotApp.DataInternal;

namespace BinanceBotInfrastructure.Services.WebsocketStorage
{
    /// <summary>
    /// Keeps 24/7 websocket connections for every user
    /// </summary>
    public class ActiveWebsockets : IActiveWebsockets
    {
        private readonly ConcurrentDictionary<int, (WebSocketWrapper prices, WebSocketWrapper price, 
            WebSocketWrapper userData)> _activeWebsocketsStorage;

        public ActiveWebsockets()
        {
            _activeWebsocketsStorage = new ConcurrentDictionary<int, (WebSocketWrapper prices, 
                WebSocketWrapper price, WebSocketWrapper userData)>();
        }

        public (WebSocketWrapper prices, WebSocketWrapper price, WebSocketWrapper userData) Get(int idUser)
        {
            var pricesStream = new ClientWebSocket();
            var priceStream = new ClientWebSocket();
            var userDataStream = new ClientWebSocket();
            return _activeWebsocketsStorage.GetOrAdd(idUser, (new WebSocketWrapper(pricesStream), 
                new WebSocketWrapper(priceStream), new WebSocketWrapper(userDataStream)));
        }

        public async Task<bool> RemoveAsync(int idUser, CancellationToken token)
        {
            var (prices, price, userData) = Get(idUser);
            await prices.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, "", token);
            prices.WebSocket.Dispose();
            await price.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, "", token);
            price.WebSocket.Dispose();
            await userData.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, "", token);
            userData.WebSocket.Dispose();
            return _activeWebsocketsStorage.TryRemove(idUser, out var result);
        }
    }
}