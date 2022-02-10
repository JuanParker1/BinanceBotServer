using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IAnalyticsService
    {
        Task<double> GetCurrentBalanceAsync(int idUSer, CancellationToken token);
        Task<TotalBalanceDto> GetTotalBalanceAsync(int idUSer, CancellationToken token);
    }
}