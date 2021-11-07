using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IWebSocketClientService _wsClientService;
        
        public CoinService(IHttpClientService httpClientService, IWebSocketClientService wsClientService)
        {
            _httpClientService = httpClientService;
            _wsClientService = wsClientService;
        }
        
        public async Task<IEnumerable<string>> GetTradingPairsAsync(CancellationToken token)
        {
            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var coinPricesInfo = 
                await _httpClientService.ProcessRequestAsync<IEnumerable<CoinPrice>>(uri, 
                    new Dictionary<string, string>(), HttpMethods.Get, token);
            
            return coinPricesInfo.Select(c => c.Symbol);
        }

        public async Task GetSubscriptionsListAsync(CancellationToken token)
        {
            var data = $"{{\"method\": \"LIST_SUBSCRIPTIONS\",\"id\": 1}}";
            
            await _wsClientService.ConnectToWebSocketAsync(new Uri("wss://stream.binance.com:9443/ws"),
                data, Console.WriteLine, token );
        }
        
        public async Task SubscribeForStreamAsync(string pair, Action<string> responseHandler, 
            CancellationToken token)
        {
            var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[\"{pair}@bookTicker\"],\"id\": 1}}";
            
            await _wsClientService.ConnectToWebSocketAsync(new Uri("wss://stream.binance.com:9443/ws"),
                data, Console.WriteLine, token );
        }
        
        public async Task SubscribeForCombinedStreamAsync(GenericCollectionDto<string> pairs, 
            Action<string> responseHandler, CancellationToken token)
        {
            var pairsString = string.Join(",", pairs.Collection.Select(p => $"\"{p}@bookTicker\""));
            var data = $"{{\"method\": \"SUBSCRIBE\",\"params\":[{pairsString}],\"id\": 1}}";
            
            await _wsClientService.ConnectToWebSocketAsync(new Uri("wss://stream.binance.com:9443/ws"),
                data, Console.WriteLine, token);
        }

        public async Task UnsubscribeFromStreamAsync(string pair, CancellationToken token)
        {
            var data = $"{{\"method\": \"UNSUBSCRIBE\",\"params\":[\"{pair}@bookTicker\"],\"id\": 1}}";
            
            await _wsClientService.ConnectToWebSocketAsync(new Uri("wss://stream.binance.com:9443/ws"),
                data, null, token);
        }
    }
}