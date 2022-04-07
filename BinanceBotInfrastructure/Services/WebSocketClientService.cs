using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Deserializers.Converters;
using BinanceBotApp.DataInternal;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Services.WebsocketStorage;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotInfrastructure.Services
{
    /// <summary>
    /// Websocket client to transfer data to the exchange back and forth
    /// </summary>
    public class WebSocketClientService : IWebSocketClientService
    {
        private readonly IHttpClientService _httpService;
        private readonly IActiveWebsockets _activeWebsockets;
        private readonly JsonSerializerOptions _jsonDeserializerOptions;

        public WebSocketClientService(IHttpClientService httpService,
            IActiveWebsockets activeWebsockets)
        {
            _httpService = httpService;
            _activeWebsockets = activeWebsockets;
            _jsonDeserializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _jsonDeserializerOptions.Converters.Add(new StringConverter());
        }
        
        public (WebSocketWrapper prices, WebSocketWrapper price, WebSocketWrapper userData) GetConnections(int idUser) =>
            _activeWebsockets.Get(idUser);

        public bool IsAlive(WebSocket webSocket) =>
            webSocket.State == WebSocketState.Open;

        public async Task<WebSocketWrapper> SendAsync(Uri endpoint, string data, int idUser, 
            WebsocketConnectionTypes streamType, CancellationToken token)
        {
            var (prices, price, userData) = GetConnections(idUser);

            var webSocketWrapper = streamType switch
            {
                WebsocketConnectionTypes.Prices => prices,
                WebsocketConnectionTypes.Price => price,
                WebsocketConnectionTypes.UserData => userData,
                _ => throw new ArgumentOutOfRangeException(nameof(WebsocketConnectionTypes),
                    "Unknown websocket connection type in Websocket client.")
            };

            var webSocket = webSocketWrapper.WebSocket;
    
            if(webSocket.State == WebSocketState.None)
                await webSocket.ConnectAsync(endpoint, token);
            
            if (webSocket.State != WebSocketState.Open)
                throw new Exception("Connection was not opened.");

            if(!string.IsNullOrEmpty(data))
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(data),
                    WebSocketMessageType.Text, true, token);
       
            return webSocketWrapper;
        }

        public async Task ListenAsync(ClientWebSocket webSocket, 
            Func<IDictionary<string, string>, Task> responseHandlerAsync, 
            CancellationToken token)
        {
            do
            {
                try
                {
                    var responseString = await GetResponseAsync(webSocket, token);
                
                    if (string.IsNullOrEmpty(responseString))
                        continue;
                
                    var response = JsonSerializer.Deserialize<IDictionary<string, string>>(responseString, 
                        _jsonDeserializerOptions) ?? new Dictionary<string, string>();
                    
                    await responseHandlerAsync.Invoke(response);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Trace.TraceError(ex.InnerException?.Message);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.Message);
                }
            } 
            while (!token.IsCancellationRequested || webSocket.State == WebSocketState.Open);
        }

        private static async Task<string> GetResponseAsync(WebSocket webSocket, 
            CancellationToken token)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            
            await using var ms = new MemoryStream();
            WebSocketReceiveResult result;
  
            do
            {
                result = await webSocket.ReceiveAsync(buffer, token);
                ms.Write(buffer.Array ?? Array.Empty<byte>(), 
                    buffer.Offset, result.Count);
                
            } while (!result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Close)
                throw new Exception("Connection closed by server side");

            ms.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(ms, Encoding.UTF8);
            var response = await reader.ReadToEndAsync();
      
            return response;
        }
    }
}