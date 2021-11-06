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
        public CoinService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }
        
        public async Task<IEnumerable<string>> GetAllAsync(CancellationToken token)
        {
            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var coinPricesInfo = 
                await _httpClientService.ProcessRequestAsync<IEnumerable<CoinPrice>>(uri, 
                    new Dictionary<string, string>(), HttpMethods.Get, token);
            
            return coinPricesInfo.Select(c => c.Symbol);
        }
        
        public async Task<CoinBestAskBidDto> GetBestPriceAsync(string symbol, 
            CancellationToken token = default)
        {
            var uri = MarketDataEndpoints.GetBestAskBidPricesEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol}
            };
            
            var bestPricesInfo = 
                await _httpClientService.ProcessRequestAsync<CoinBestAskBidDto>(uri, 
                    qParams, HttpMethods.Get, token);

            return bestPricesInfo;
        }
    }
}