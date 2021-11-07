using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ICoinService
    {
        Task<IEnumerable<string>> GetAllAsync(CancellationToken token = default);
        Task GetPairBestPriceAsync(string pair, Action<string> responseHandler, 
            CancellationToken token);
        Task GetPairsBestPricesAsync(GenericCollectionDto<string> pairs, 
            Action<string> responseHandler, CancellationToken token);
    }
}