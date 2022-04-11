using System;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data.Analytics;

namespace BinanceBotApp.Services
{
    public interface IAnalyticsService
    {
        Task<ProfitToBtcDto> GetProfitToBtcAsync(int idUser, DateTime intervalStart,
            DateTime intervalEnd, CancellationToken token);
        Task<TradeTypesStatsDto> GetTradeTypesStatsAsync(int idUser, DateTime intervalStart,
            DateTime intervalEnd, CancellationToken token);
    }
}