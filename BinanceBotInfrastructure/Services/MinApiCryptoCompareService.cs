using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data.Analytics;
using BinanceBotApp.Services;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotInfrastructure.Extensions;

namespace BinanceBotInfrastructure.Services;

public class MinApiCryptoCompareService : ICryptoInfoService
{
    private readonly IHttpClientService _httpService;

    public MinApiCryptoCompareService(IHttpClientService httpService)
    {
        _httpService = httpService;
    }
    
    public async Task<IEnumerable<ProfitToBtcHistoryDto>> GetPriceHistoryAsync(string symbol, 
        DateTime intervalStart, DateTime intervalEnd, CancellationToken token)
    {
        var intervalDays = (int)(DateTime.Now - intervalStart).TotalDays;
        
        if (intervalDays < 1)
            intervalDays = 1;
            
        var btcPriceRequestUrl = GetCoinPriceApiUrl(symbol, intervalDays);

        var btcPriceHistory = await _httpService.GetRequestAsync<PriceApiResponse>(btcPriceRequestUrl,
            token);

        return btcPriceHistory.Data.Data
            .Where(h => DateTime.Now.FromUnixTimeSeconds(h.Time) < intervalEnd)
            .Select(h => 
            new ProfitToBtcHistoryDto
            {
                Date = DateTime.Now.FromUnixTimeSeconds(h.Time),
                BtcPrice = h.High,
                Profit = 0
            }
        );
    }
    
    private static string GetCoinPriceApiUrl(string symbol, int intervalDays) =>
        $"https://min-api.cryptocompare.com/data/v2/histoday?e=Binance&fsym={symbol.ToUpper()}&tsym=USDT&limit={intervalDays}";
}