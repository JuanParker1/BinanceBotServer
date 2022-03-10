using System.Net.WebSockets;

namespace BinanceBotApp.DataInternal
{
    public class WebSocketWrapper
    {
        public bool IsListening;
        public ClientWebSocket WebSocket { get; }

        public WebSocketWrapper(ClientWebSocket webSocket)
        {
            WebSocket = webSocket;
        }
    }
}