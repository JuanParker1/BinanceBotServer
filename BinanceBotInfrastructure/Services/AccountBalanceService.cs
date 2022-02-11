using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.DataInternal.Deserializers;

namespace BinanceBotInfrastructure.Services
{
    public class AccountBalanceService : CrudService<BalanceChangeDto, BalanceChange>, 
        IAccountBalanceService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;

        public AccountBalanceService(IBinanceBotDbContext db, ISettingsService settingsService, 
            IHttpClientService httpService) : base(db)
        {
            _db = db;
            _settingsService = settingsService;
            _httpService = httpService;
        }
        
        public async Task<IEnumerable<CoinAmountDto>> GetCurrentBalanceAsync(int idUser, 
            CancellationToken token)
        {
            var apiKeys = await _settingsService.GetApiKeysAsync(idUser,
                token);

            if (apiKeys == default)
                return null;

            var uri = AccountEndpoints.GetAccountInformationEndpoint();
            
            var qParams = new Dictionary<string, string>()
            {
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var allCoins = 
                await _httpService.ProcessRequestAsync<AccountBalanceInfo>(uri, 
                    qParams, apiKeys, HttpMethods.SignedGet, token);

            var currentBalance = allCoins.Balances?.Where(c => 
                c.Free > 0 || c.Locked > 0);
            
            return currentBalance; 
        }
        
        public async Task<BalanceSummaryDto> GetTotalBalanceAsync(int idUser, 
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
            
            var totalBalance = new BalanceSummaryDto()
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