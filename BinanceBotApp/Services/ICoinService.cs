using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ICoinService
    {
        Task<IEnumerable<string>> GetTradingPairsAsync(int idUser, 
            CancellationToken token);
        Task GetCoinPriceStreamAsync(string pair, int idUser,  
            Action<string> responseHandler, CancellationToken token);
        Task GetCoinPricesStreamAsync(GenericCollectionDto<string> pairs, 
            int idUser, Action<string> responseHandler, 
            CancellationToken token);
        Task UnsubscribeCoinPriceStreamAsync(GenericCollectionDto<string> pairs, int idUser,  
            CancellationToken token);
    }
}