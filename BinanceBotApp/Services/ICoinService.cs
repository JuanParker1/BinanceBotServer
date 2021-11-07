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
        Task SubscribeForStreamAsync(string pair, Action<string> responseHandler, 
            CancellationToken token);
        Task SubscribeForCombinedStreamAsync(GenericCollectionDto<string> pairs, 
            Action<string> responseHandler, CancellationToken token);
        Task UnsubscribeFromStreamAsync(string pair, CancellationToken token);
    }
}