using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        public async Task<double> GetCurrentBalanceAsync(int idUSer, CancellationToken token)
        {
            return await Task.FromResult(1500);
        }
        
        public async Task<TotalBalanceDto> GetTotalBalanceAsync(int idUSer, CancellationToken token)
        {
            var totalBalance = new TotalBalanceDto()
            {
                TotalDeposit = 1400,
                TotalWithdraw = 1200
            };

            return await Task.FromResult(totalBalance);
        }
    }
}