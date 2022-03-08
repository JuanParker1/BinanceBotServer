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

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;
        private readonly IWebSocketClientService _wsService;
        
        public CoinService(ISettingsService settingsService, 
            IHttpClientService httpService, 
            IWebSocketClientService wsService)
        {
            _settingsService = settingsService;
            _httpService = httpService;
            _wsService = wsService;
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

        public async Task GetSubscriptionsListAsync(CancellationToken token)
        {
            var data = $"{{\"method\": \"LIST_SUBSCRIPTIONS\",\"id\": 1}}";
            
            await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, Console.WriteLine, token ); // TODO: Handler надо принять из контроллера
        }
        
        public async Task GetCoinPriceStreamAsync(string pair, Action<string> responseHandler, 
            CancellationToken token)
        {
            var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[\"{pair}@bookTicker\"],\"id\": 1}}";
            
            await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, Console.WriteLine, token );
        }
        
        public async Task GetCoinPricesStreamAsync(GenericCollectionDto<string> pairs, 
            Action<string> responseHandler, CancellationToken token)
        {
            var pairsString = string.Join(",", pairs.Collection.Select(p => $"\"{p}@bookTicker\""));
            var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";
            
            await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, responseHandler, token);
        }

        public async Task UnsubscribeCoinPriceStreamAsync(string pair, CancellationToken token)
        {
            var data = $"{{\"method\": \"UNSUBSCRIBE\",\"params\":[\"{pair}@bookTicker\"],\"id\": 1}}";
            
            await _wsService.ConnectToWebSocketAsync(TradeWebSocketEndpoints.GetMainWebSocketEndpoint(),
                data, null, token);
        }

        private string CutTradePairEnding(string tradePair)
        {
            if (tradePair.EndsWith("BTC") || tradePair.EndsWith("BNB"))
                return tradePair[..^3];
            if (tradePair.EndsWith("USDT"))
                return tradePair[..^4];
            return tradePair[..^3];
        }
    }
}