using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotInfrastructure.Services.CoinPricesStorage;

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;
        private readonly IWebSocketClientService _webSocketService;
        private readonly ICoinPricesStorage _coinPricesStorage;

        public CoinService(ISettingsService settingsService, IHttpClientService httpService, 
            IWebSocketClientService webSocketService, ICoinPricesStorage coinPricesStorage)
        {
            _settingsService = settingsService;
            _httpService = httpService;
            _webSocketService = webSocketService;
            _coinPricesStorage = coinPricesStorage;
        }
        
        public async Task<IEnumerable<string>> GetTradingPairsAsync(int idUser, 
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);

            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var coinPricesInfo = 
                await _httpService.ProcessRequestAsync<IEnumerable<CoinPrice>>(uri, 
                    new Dictionary<string, string>(), keys,HttpMethods.Get, token);
            
            var filtered =  coinPricesInfo.Select(c => CutTradePairEnding(c.Symbol))
                .Distinct();
            
            return filtered;
        }
        
        public Task SubscribeSingleCoinPriceStreamAsync(string pair, int idUser, 
            Action<string> responseHandler, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[\"{pair.ToLower()}@bookTicker\"],\"id\": 1}}";

                var endpoint = TradeWebSocketEndpoints.GetMainWebSocketEndpoint();
            
                var wsClientWrapper = await _webSocketService.SendAsync(endpoint, data, 
                    idUser, WebsocketConnectionTypes.Price, token);
                
                var userCoinPrices = _coinPricesStorage.Get(idUser);

                if (!wsClientWrapper.IsListening)
                {
                    wsClientWrapper.IsListening = true;
                    await _webSocketService.ListenAsync(idUser, wsClientWrapper.WebSocket, 
                        userCoinPrices, responseHandler, token);
                }
            }, token);
        }

        public Task SubscribeCoinPricesStreamAsync(IEnumerable<string> pairs, int idUser, 
            Action<string> responseHandler, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var pairsString = string.Join(",", pairs.Select(p => $"\"{p.ToLower()}@bookTicker\""));
                var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";

                var endpoint = TradeWebSocketEndpoints.GetMainWebSocketEndpoint();
            
                var wsClientWrapper = await _webSocketService.SendAsync(endpoint, data, 
                    idUser, WebsocketConnectionTypes.Prices, token);
                
                var userCoinPrices = _coinPricesStorage.Get(idUser);

                if (!wsClientWrapper.IsListening)
                {
                    wsClientWrapper.IsListening = true;
                    await _webSocketService.ListenAsync(idUser, wsClientWrapper.WebSocket, 
                        userCoinPrices, responseHandler, token);
                }
            }, token);
        }

        public async Task UnsubscribeCoinPriceStreamAsync(IEnumerable<string> pairs, 
            int idUser, WebsocketConnectionTypes connectionType, CancellationToken token)
        {
            var pairsString = string.Join(",", pairs.Select(p => $"\"{p.ToLower()}@bookTicker\""));
            var data = $"{{\"method\": \"UNSUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";

            await _webSocketService.SendAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, connectionType, token);
        }

        public async Task GetSubscriptionsListAsync(int idUser, CancellationToken token)
        {
            var data = $"{{\"method\": \"LIST_SUBSCRIPTIONS\",\"id\": 1}}";   // TODO: Попробовать через @ и '' обычные кавычки
            
            await _webSocketService.SendAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.Prices, token);
        }

        private static string CutTradePairEnding(string tradePair) =>
            tradePair.EndsWith("USDT") 
                ? tradePair[..^4] 
                : tradePair[..^3];
    }
}