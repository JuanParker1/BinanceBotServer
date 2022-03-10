using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;
        private readonly IWebSocketClientService _webSocketService;

        public CoinService(ISettingsService settingsService, IHttpClientService httpService, 
            IWebSocketClientService webSocketService)
        {
            _settingsService = settingsService;
            _httpService = httpService;
            _webSocketService = webSocketService;
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

        public async Task SubscribeCoinPricesStreamAsync(IEnumerable<string> pairs, int idUser, 
            Action<string> responseHandler, CancellationToken token)
        {
            var pairsString = string.Join(",", pairs.Select(p => $"\"{p}@bookTicker\""));
            var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";
            
            var wsClientWrapper = await _webSocketService.SendAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.Prices, token);

            if (!wsClientWrapper.IsListening)
            {
                wsClientWrapper.IsListening = true;
                await _webSocketService.ListenAsync(wsClientWrapper.WebSocket, 
                    responseHandler, token);
            }

        }

        public async Task UnsubscribeCoinPriceStreamAsync(IEnumerable<string> pairs, int idUser,  
            CancellationToken token)
        {
            var pairsString = string.Join(",", pairs.Select(p => $"\"{p}@bookTicker\""));
            var data = $"{{\"method\": \"UNSUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";

            await _webSocketService.SendAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, idUser, WebsocketConnectionTypes.Prices, token);
        }

        private static string CutTradePairEnding(string tradePair) =>
            tradePair.EndsWith("USDT") 
                ? tradePair[..^4] 
                : tradePair[..^3];
    }
}