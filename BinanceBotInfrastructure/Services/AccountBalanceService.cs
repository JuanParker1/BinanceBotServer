using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        public async Task<double> GetCurrentBalanceAsync(int idUser, CancellationToken token)
        {
            return await Task.FromResult(1500); // TODO: Запрос к БД where idUser, groupby IdDirection. Далее возвращать тапл с мин и макс и их вычитать
        }
        
        public async Task<TotalBalanceDto> GetTotalBalanceAsync(int idUser, CancellationToken token)
        {
            var totalBalance = new TotalBalanceDto()
            {
                TotalDeposit = 1400, // Тут то же самое, как верху
                TotalWithdraw = 1200
            };

            return await Task.FromResult(totalBalance);
        }
    }
}