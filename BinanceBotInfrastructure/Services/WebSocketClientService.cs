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
        private readonly ISettingsService _settingsService;
        private readonly IActiveWebsockets _activeWebsockets;
        private readonly JsonSerializerOptions _jsonDeserializerOptions;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private const int _notifyThreshold = 20;

        public WebSocketClientService(IHttpClientService httpService, 
            ISettingsService settingsService, IActiveWebsockets activeWebsockets)
        {
            _httpService = httpService;
            _settingsService = settingsService;
            _activeWebsockets = activeWebsockets;
            _jsonDeserializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _jsonDeserializerOptions.Converters.Add(new StringConverter());
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
        
        public (WebSocketWrapper prices, WebSocketWrapper userData) GetConnections(int idUser) =>
            _activeWebsockets.Get(idUser);

        public bool IsAlive(WebSocket webSocket) =>
            webSocket.State == WebSocketState.Open;

        public async Task<WebSocketWrapper> SendAsync(Uri endpoint, string data, int idUser, 
            WebsocketConnectionTypes streamType, CancellationToken token)
        {
            var (prices, userData) = GetConnections(idUser);

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

        public async Task ListenAsync(int idUser, ClientWebSocket webSocket, 
            IDictionary<string, double> highestPrices, Action<string> responseHandler, 
            CancellationToken token)
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
                        _jsonDeserializerOptions) ?? new Dictionary<string, string>();

                    if (response.ContainsKey("s") && !string.IsNullOrEmpty(response["s"]))
                    {
                        i++;
                        if (i <= _notifyThreshold) 
                            continue;
                        await HandleNewCoinPriceAsync(idUser, response, highestPrices, 
                            responseHandler, token);
                        i = 0;
                    }
                    
                    if(response.ContainsKey("result"))
                        responseHandler?.Invoke(JsonSerializer.Serialize(new { Result = response["result"] },
                            _jsonSerializerOptions));
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
        
        public async Task<string> GetListenKey(CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
        
            var listenKey = await _httpService.ProcessRequestAsync<string>(uri,
                null, default, HttpMethods.SignedPost, token);
        
            return listenKey;
        }
        
        public async Task ExtendListenKey(string listenKey, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
        
            await _httpService.ProcessRequestAsync<string>(uri,
                new Dictionary<string, string>(), 
                default, HttpMethods.SignedPut, token);
        }
        
        public async Task DeleteListenKey(string listenKey, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
        
            await _httpService.ProcessRequestAsync<string>(uri,
                new Dictionary<string, string>(), 
                default, HttpMethods.SignedDelete, token);
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

        private async Task HandleNewCoinPriceAsync(int idUser, IDictionary<string, string> response,
            IDictionary<string, double> highestPrices, Action<string> responseHandler,
            CancellationToken token)
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

            var userSettings = await _settingsService.GetSettingsAsync(idUser, token);

            if (userSettings.IsTradeEnabled)
            {
                //TODO: Обновить стоп ордер на бирже, если включена торговля
            }

            responseHandler?.Invoke(JsonSerializer.Serialize(new { Symbol = tradePair, Price = currentPrice },
                _jsonSerializerOptions));
        }
    }
}