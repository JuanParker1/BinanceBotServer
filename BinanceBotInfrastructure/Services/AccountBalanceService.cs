using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services
{
    public class AccountBalanceService : CrudService<BalanceChangeDto, BalanceChange>, 
        IAccountBalanceService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly ISettingsService _settingsService;

        public AccountBalanceService(IBinanceBotDbContext db,
            ISettingsService settingsService) : base(db)
        {
            _db = db;
            _settingsService = settingsService;
        }
        
        public async Task<double?> GetCurrentBalanceAsync(int idUser, 
            CancellationToken token)
        {
            var apiKeys = await _settingsService.GetApiKeysAsync(idUser,
                token);

            if (apiKeys == default)
                return null;
            
            // Запрос к апи на юзеринфо
            
            return await Task.FromResult(1500); 
        }
        
        public async Task<TotalBalanceDto> GetTotalBalanceAsync(int idUser, 
            CancellationToken token)
        {
            var changesSum = await (from bChange in _db.BalanceChanges
                                    where bChange.IdUser == idUser
                                    group bChange by bChange.IdDirection into g
                                    select new
                                    {
                                        IdDirection = g.Key,
                                        Sum = g.Sum(b => b.Amount)
                                    })
                                    .ToListAsync(token);
            
            var totalBalance = new TotalBalanceDto()
            {
                TotalDeposit = changesSum
                    .FirstOrDefault(c => c.IdDirection == 1)?.Sum,
                TotalWithdraw = changesSum
                    .FirstOrDefault(c => c.IdDirection == 2)?.Sum,
            };

            return totalBalance;
        }
    }
}