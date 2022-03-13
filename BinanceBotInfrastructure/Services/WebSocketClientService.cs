using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Services.WebsocketStorage;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotInfrastructure.Services
{
    public class WebSocketClientService : IWebSocketClientService
    {
        private readonly IActiveWebsockets _activeWebsockets;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private const int _notifyThreshold = 10;

        public WebSocketClientService(IActiveWebsockets activeWebsockets)
        {
            _activeWebsockets = activeWebsockets;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _jsonSerializerOptions.Converters.Add(new StringConverter());
        }

        public async Task<WebSocketWrapper> SendAsync(Uri endpoint, string data, int idUser, 
            WebsocketConnectionTypes streamType, CancellationToken token)
        {
            var (prices, userData) = _activeWebsockets.Get(idUser);

            var webSocketWrapper = streamType switch
            {
                WebsocketConnectionTypes.Prices => prices,
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

        public async Task ListenAsync(ClientWebSocket webSocket, IDictionary<string, double> highestPrices, 
            Action<string> responseHandler, CancellationToken token)
        {
            var i = 0;
            do
            {
                try
                {
                    var responseString = await GetResponseAsync(webSocket, token);
                
                    if (string.IsNullOrEmpty(responseString))
                        continue;
                
                    var response = JsonSerializer.Deserialize<IDictionary<string, string>>(responseString, 
                        _jsonSerializerOptions) ?? new Dictionary<string, string>();

                    if (response.ContainsKey("s") && !string.IsNullOrEmpty(response["s"]))
                    {
                        i++;
                        if (i <= _notifyThreshold) 
                            continue;
                        HandleNewCoinPrice(response, highestPrices, responseHandler);
                        i = 0;
                    }
                    
                    if(response.ContainsKey("result"))
                        responseHandler?.Invoke(response["result"]);
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

        private static void HandleNewCoinPrice(IDictionary<string, string> response,
            IDictionary<string, double> highestPrices, Action<string> responseHandler)
        {
            if(!response.ContainsKey("s") || string.IsNullOrEmpty(response["s"]))
                return;
            
            var tradePair = response["s"];
     
            if (!double.TryParse(response["b"], out var currentPrice))
                return;
            
            var currentHighestPrice = highestPrices.ContainsKey(tradePair) 
                ? highestPrices[tradePair] 
                : 0D;

            if (currentPrice > currentHighestPrice)
                highestPrices[tradePair] = currentPrice;

            //TODO: Обновить стоп ордер на бирже.
            
            responseHandler?.Invoke(response["b"]);
        }
    }
}