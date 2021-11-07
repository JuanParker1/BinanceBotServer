using System;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    /// <summary>
    /// Web socket client for Binance streams 
    /// </summary>
    public class WebSocketClientService : IWebSocketClientService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly Dictionary<int, WebSocket> _activeWebSockets;
        private int _idWebSocket = 0;

        public WebSocketClientService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _activeWebSockets = new Dictionary<int, WebSocket>();
        }

        /// <summary>
        /// Get stream listen key. Used to connect to Binance streams.
        /// </summary>
        /// <param name="token">Current task cancellation token</param>
        /// <returns>Listen key string</returns>
        public async Task<string> GetListenKey(CancellationToken token)
        {
            var streamResponse = await _httpClientService.ProcessRequestAsync<string>(
                    UserDataWebSocketEndpoints.GetListenKeyEndpoint(), null, 
                    HttpMethods.SignedPost,token)
                .ConfigureAwait(false);
            
            return streamResponse;
        }
        
        /// <summary>
        /// Connects to Binance streams.
        /// </summary>
        /// <param name="endpoint">Endpoint to connect to</param>
        /// <param name="data">Data to send</param>
        /// <param name="responseHandler">Method to handle stream response</param>
        /// <param name="token">Current task cancellation token</param>
        /// <returns></returns>
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

        /// <summary>
        /// Close a specific WebSocket instance using the int provided on creation
        /// </summary>
        /// <param name="id">Connection id</param>
        /// <param name="token">Current task cancellation token</param>
        /// <returns>Connection was closed or not (bool)</returns>
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

        /// <summary>
        /// Checks whether a specific WebSocket instance is active or
        /// not using the Guid provided on creation
        /// </summary>
        /// <param name="id">Connection id</param>
        /// <returns>Connection is opened or not (bool)</returns>
        public bool IsAlive(int id)
        {
            if (!_activeWebSockets.ContainsKey(id)) 
                throw new Exception($"No Websocket exists with the Id {id}");
            
            var ws = _activeWebSockets[id];
            return ws.State == WebSocketState.Open;
        }
    }
}