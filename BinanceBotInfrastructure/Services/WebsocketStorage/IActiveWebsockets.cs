using System.Net.WebSockets;

namespace BinanceBotInfrastructure.Services.WebsocketStorage
{
    public interface IActiveWebsockets
    {
        (ClientWebSocket prices, ClientWebSocket userData) Get(int idUser);
        bool Remove(int idUser);
    }
}