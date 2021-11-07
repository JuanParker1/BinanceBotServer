using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ICoinService
    {
        Task<IEnumerable<string>> GetAllAsync(CancellationToken token = default);
        Task<CoinBestAskBidDto> GetBestPriceAsync(string symbol, 
            CancellationToken token = default);
        Task ConnectToWebSocketAsync(CancellationToken token);
    }
}