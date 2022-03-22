using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotApp.Services
{
    public interface ICoinService
    {
        Task<IEnumerable<string>> GetTradingPairsAsync(int idUser, 
            CancellationToken token);
        Task SubscribeSingleCoinPriceStreamAsync(string pair, int idUser, 
            Action<string> responseHandler, CancellationToken token);
        Task SubscribeCoinPricesStreamAsync(IEnumerable<string> pairs, int idUser, 
            Action<string> responseHandler, CancellationToken token);
        Task UnsubscribeCoinPriceStreamAsync(IEnumerable<string> pairs, 
            int idUser, WebsocketConnectionTypes connectionType, 
            CancellationToken token);
        Task GetSubscriptionsListAsync(int idUser, CancellationToken token);
    }
}