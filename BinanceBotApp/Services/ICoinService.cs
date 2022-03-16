using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface ICoinService
    {
        Task<IEnumerable<string>> GetTradingPairsAsync(int idUser, 
            CancellationToken token);
        void SubscribeCoinPricesStream(IEnumerable<string> pairs, 
            int idUser, Action<string> responseHandler);
        Task UnsubscribeCoinPriceStreamAsync(IEnumerable<string> pairs, int idUser,  
            CancellationToken token);
        Task GetSubscriptionsListAsync(int idUser, CancellationToken token);
    }
}