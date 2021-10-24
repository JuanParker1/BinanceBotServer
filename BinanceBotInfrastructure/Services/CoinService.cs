using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
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
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CoinService(IBinanceHttpClient client)
        {
            _client = client;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
        
        public async Task<IEnumerable<string>> GetAllAsync(CancellationToken token = default)
        {
            var uri = MarketDataEndpoints.GetCoinsPricesEndpoint();
            var response = await _client.GetRequestAsync(uri,
                null, token);
            
            var coinPrices = await HandleResponseAsync<IEnumerable<CoinPrice>>(response, token);
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
            
            var coinInfo = await HandleResponseAsync<CoinBestAskBidDto>(response, token);
            return coinInfo;
        }
        
        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage message, 
            CancellationToken token) where T : class
        {
            if (message is null) 
                throw new ArgumentNullException("Message is null");
            
            if (!message.IsSuccessStatusCode) 
                throw new Exception($"Request not successfull. " +
                                    $"Error status code: {message.StatusCode}");
            
            var messageJson = await message.Content.ReadAsStringAsync(token);

            var messageObject = JsonSerializer.Deserialize<T>(messageJson, _jsonSerializerOptions);
            if (messageObject is not null) 
                return messageObject;
            
            var deserializeErrorMessage = "Unable to deserialize http message to: " +
                                          $"{nameof(T)}.";
            throw new Exception(deserializeErrorMessage);
        }
    }
}