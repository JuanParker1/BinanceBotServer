using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Deserializers;

namespace BinanceBotApp.Services
{
    public interface IAccountBalanceService
    {
        Task<IEnumerable<CoinAmountDto>> GetCurrentBalanceAsync(int idUser, CancellationToken token);
        Task<BalanceSummaryDto> GetTotalBalanceAsync(int idUser, CancellationToken token);
    }
}