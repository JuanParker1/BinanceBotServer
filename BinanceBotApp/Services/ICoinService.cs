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
        Task SubscribeCoinPricesStreamAsync(IEnumerable<string> pairs, 
            int idUser, Action<string> responseHandler, 
            CancellationToken token);
        Task UnsubscribeCoinPriceStreamAsync(IEnumerable<string> pairs, int idUser,  
            CancellationToken token);
    }
}