using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IAccountBalanceService : ICrudService<BalanceChangeDto>
    {
        Task<double?> GetCurrentBalanceAsync(int idUser, CancellationToken token);
        Task<TotalBalanceDto> GetTotalBalanceAsync(int idUser, CancellationToken token);
    }
}