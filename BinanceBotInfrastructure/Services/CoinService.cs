using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;
using BinanceAPI.Endpoints;
using BinanceAPI.Clients.Interfaces;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal;

namespace BinanceBotInfrastructure.Services
{
    public class CoinService : ICoinService
    {
        private readonly IBinanceHttpClient _client;
        private readonly IHttpResponseService _responseService;
        public CoinService(IBinanceHttpClient client, IHttpResponseService responseService)
        {
            _client = client;
            _responseService = responseService;
        }
        
        public async Task<IEnumerable<string>> GetAllAsync(CancellationToken token)
        {
            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var response = await _client.GetRequestAsync(uri,
                null, token);
            
            var coinPrices = await 
                _responseService.HandleResponseAsync<IEnumerable<CoinPrice>>(response, 
                    token);
            return coinPrices.Select(c => c.Symbol);
        }
        
        public async Task<CoinBestAskBidDto> GetBestPriceAsync(string symbol, 
            CancellationToken token = default)
        {
            var uri = MarketDataEndpoints.GetBestAskBidPricesEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol}
            };
            var response = await _client.GetRequestAsync(uri, qParams, token);
            
            var coinInfo = await 
                _responseService.HandleResponseAsync<CoinBestAskBidDto>(response, 
                    token);
            return coinInfo;
        }
    }
}