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

        public bool Remove(int idUser)
        {
            var (prices, userData) = Get(idUser);
            prices.Dispose();
            userData.Dispose();
            return _activeWebsocketsStorage.TryRemove(idUser, out var result);
        }
    }
}