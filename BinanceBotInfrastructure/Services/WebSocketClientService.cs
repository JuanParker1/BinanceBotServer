using System;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    public class WebSocketClientService : IWebSocketClientService
    {
        private readonly Dictionary<int, WebSocket> _activeWebSockets;
        private int _idWebSocket = 0;

        public WebSocketClientService() =>
            _activeWebSockets = new Dictionary<int, WebSocket>();
        

        public async Task ConnectToWebSocketAsync(Uri endpoint, string data,
            Action<string> responseHandler, CancellationToken token)
        {
            using var webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(endpoint, token).ConfigureAwait(false);

            if (webSocket.State != WebSocketState.Open)
                throw new Exception("Connection was not opened.");
            
            _activeWebSockets.Add(_idWebSocket, webSocket);
            _idWebSocket++;
            
            if(!string.IsNullOrEmpty(data))
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(data), 
                    WebSocketMessageType.Text, true, token)
                    .ConfigureAwait(false);
            
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                await using var ms = new MemoryStream();
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, CancellationToken.None)
                        .ConfigureAwait(false);
                    ms.Write(buffer.Array ?? Array.Empty<byte>(), buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    throw new Exception("Connection closed by server side");

                ms.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(ms, Encoding.UTF8);
                var response = await reader.ReadToEndAsync().ConfigureAwait(false);
                responseHandler?.Invoke(response);
                
            } while (!token.IsCancellationRequested);
        }
        
        public async Task<bool> CloseWebSocketInstance(int id,
            CancellationToken token)
        {
            if(!_activeWebSockets.ContainsKey(id))
                throw new Exception($"No Websocket exists with the Id {id}");
            
            var ws = _activeWebSockets[id];
            await ws.CloseAsync(WebSocketCloseStatus.Empty, "", token)
                .ConfigureAwait(false);
            return _activeWebSockets.Remove(id);
        }
        
        public bool IsAlive(int id)
        {
            if (!_activeWebSockets.ContainsKey(id)) 
                throw new Exception($"No Websocket exists with the Id {id}");
            
            var ws = _activeWebSockets[id];
            return ws.State == WebSocketState.Open;
        }
    }
}