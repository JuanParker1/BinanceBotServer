using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotInfrastructure.Services.WebsocketStorage;
using System.Net.WebSockets;
using System.Text;

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;
        private readonly IWebSocketClientService _wsService;
        private readonly IActiveWebsockets _activeWebsockets; // TODO: Убрать.

        public CoinService(ISettingsService settingsService, IHttpClientService httpService, 
            IWebSocketClientService wsService, IActiveWebsockets activeWebsockets)
        {
            _settingsService = settingsService;
            _httpService = httpService;
            _wsService = wsService;
            _activeWebsockets = activeWebsockets;
        }
        
        public async Task<IEnumerable<string>> GetTradingPairsAsync(int idUser, 
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);

            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var coinPricesInfo = 
                await _httpService.ProcessRequestAsync<IEnumerable<CoinInfo>>(uri, 
                    new Dictionary<string, string>(), keys,HttpMethods.Get, token);
            
            var filtered =  coinPricesInfo.Select(c => CutTradePairEnding(c.Symbol))
                .Distinct();
            
            return filtered;
        }

        public async Task GetCoinPriceStreamAsync(string pair, int idUser,  Action<string> responseHandler, 
            CancellationToken token)
        {
            var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[\"{pair}@bookTicker\"],\"id\": 1}}";
            
            await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.Prices, Console.WriteLine, token );
        }
        
        public async Task GetCoinPricesStreamAsync(GenericCollectionDto<string> pairs, int idUser, 
            Action<string> responseHandler, CancellationToken token)
        {
            var pairsString = string.Join(",", pairs.Collection.Select(p => $"\"{p}@bookTicker\""));
            var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";
            
            await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.Prices, responseHandler, token);
        }

        public async Task UnsubscribeCoinPriceStreamAsync(GenericCollectionDto<string> pairs, int idUser,  
            CancellationToken token)
        {
            var wsClient = _activeWebsockets.Get(idUser).prices;

            var pairsString = string.Join(",", pairs.Collection.Select(p => $"\"{p}@bookTicker\""));
            var data = $"{{\"method\": \"UNSUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";
            
            await wsClient.SendAsync(Encoding.UTF8.GetBytes(data), // TODO: В сервисе разбить на методы Send и Listen. Тут эксупшон, он если поменять как надо он пытается там снова коннектится, хотя уже законнекчен.
                WebSocketMessageType.Text, true, token); 
            
            // await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
            //     data, idUser, WebsocketConnectionTypes.Prices, null, token);
        }

        private static string CutTradePairEnding(string tradePair) =>
            tradePair.EndsWith("USDT") 
                ? tradePair[..^4] 
                : tradePair[..^3];
    }
}