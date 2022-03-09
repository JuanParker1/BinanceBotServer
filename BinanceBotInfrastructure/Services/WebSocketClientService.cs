using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Services.WebsocketStorage;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotInfrastructure.Services
{
    public class WebSocketClientService : IWebSocketClientService
    {
        private readonly IActiveWebsockets _activeWebsockets;

        public WebSocketClientService(IActiveWebsockets activeWebsockets)
        {
            _activeWebsockets = activeWebsockets;
        }

        public async Task<ClientWebSocket> SendAsync(Uri endpoint, string data, int idUser, 
            WebsocketConnectionTypes streamType, CancellationToken token)
        {
            var (prices, userData) = _activeWebsockets.Get(idUser);

            var webSocket = streamType switch
            {
                WebsocketConnectionTypes.Prices => prices,
                WebsocketConnectionTypes.UserData => userData,
                _ => throw new ArgumentOutOfRangeException(nameof(WebsocketConnectionTypes),
                    "Unknown websocket connection type in Websocket client.")
            };
            
            if(webSocket.State != WebSocketState.Open)
                await webSocket.ConnectAsync(endpoint, token);

            if (webSocket.State != WebSocketState.Open)
                throw new Exception("Connection was not opened.");

            if(!string.IsNullOrEmpty(data))
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(data),
                    WebSocketMessageType.Text, true, token);

            return webSocket;
        }

        public async Task ListenAsync(ClientWebSocket webSocket, Action<string> responseHandler, 
            CancellationToken token)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);

            do
            {
                await using var ms = new MemoryStream();
                WebSocketReceiveResult result;
      
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array ?? Array.Empty<byte>(), buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    throw new Exception("Connection closed by server side");

                ms.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(ms, Encoding.UTF8);
                var response = await reader.ReadToEndAsync();

                responseHandler?.Invoke(response);

            } while (!token.IsCancellationRequested || webSocket.State == WebSocketState.Open);
        }
    }
}