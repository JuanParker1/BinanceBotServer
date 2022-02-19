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
using Mapster;

namespace BinanceBotInfrastructure.Services
{
    public class AccountBalanceService : CrudService<BalanceChangeDto, BalanceChange>, 
        IAccountBalanceService
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;

        public AccountBalanceService(IBinanceBotDbContext db, ISettingsService settingsService, 
            IHttpClientService httpService) : base(db)
        {
            _settingsService = settingsService;
            _httpService = httpService;
        }

        public async Task<IEnumerable<BalanceChangeDto>> GetAllAsync(int idUser,
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token)
        {
            var start = DateTime.MinValue;
            var end = DateTime.Now;

            if (intervalStart != default)
                start = intervalStart;

            if (intervalEnd != default)
                end = intervalEnd;

            var query = from bChange in Db.BalanceChanges
                        where bChange.IdUser == idUser &&
                              bChange.Date >= start &&
                              bChange.Date <= end
                        select bChange;
            
            var history = await query.ToListAsync(token);

            var historyDtos = history.Select(h =>
            {
                var dto = h.Adapt<BalanceChangeDto>();
                dto.Direction = h.IdDirection == 1 
                    ? "Пополнение" 
                    : "Вывод";
                return dto;
            });

            return historyDtos;
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
            var changesSum = await (from bChange in Db.BalanceChanges
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