using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;
using BinanceAPI.Endpoints;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly IHttpResponseService _responseService;
        public CoinService(IHttpResponseService responseService)
        {
            _responseService = responseService;
        }
        
        public async Task<IEnumerable<string>> GetAllAsync(CancellationToken token)
        {
            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var coinPricesInfo = 
                await _responseService.ProcessRequestAsync<IEnumerable<CoinPrice>>(uri, 
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
                await _responseService.ProcessRequestAsync<CoinBestAskBidDto>(uri, 
                    qParams, HttpMethods.Get, token);

            return bestPricesInfo;
        }
    }
}