using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ICoinService
    {
        Task<IEnumerable<string>> GetTradingPairsAsync(CancellationToken token = default);
        Task GetSubscriptionsListAsync(CancellationToken token);
        Task GetCoinPriceStreamAsync(string pair, Action<string> responseHandler, 
            CancellationToken token);
        Task GetCoinsListPriceStreamAsync(GenericCollectionDto<string> pairs, 
            Action<string> responseHandler, CancellationToken token);
        Task UnsubscribeCoinPriceStreamAsync(string pair, CancellationToken token);
    }
}