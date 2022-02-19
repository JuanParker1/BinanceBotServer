using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IAccountBalanceService
    {
        Task<IEnumerable<BalanceChangeDto>> GetAllAsync(int idUser,
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token);
        Task<IEnumerable<CoinAmountDto>> GetCurrentBalanceAsync(int idUser, 
            CancellationToken token);
        Task<BalanceSummaryDto> GetTotalBalanceAsync(int idUser, 
            CancellationToken token);
    }
}