using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using BinanceBotApp.Data.Analytics;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly ICryptoInfoService _cryptoInfoService;

        public AnalyticsService(IBinanceBotDbContext db,
            ICryptoInfoService cryptoInfoService)
        {
            _db = db;
            _cryptoInfoService = cryptoInfoService;
        }

        public async Task<ProfitToBtcDto> GetProfitToBtcAsync(int idUser, DateTime intervalStart, 
            DateTime intervalEnd, CancellationToken token)
        {
            var start = DateTime.MinValue;
            var end = DateTime.Now;

            if (intervalStart != default)
                start = intervalStart;

            if (intervalEnd != default)
                end = intervalEnd;

            var btcPriceHistory = await _cryptoInfoService.GetPriceHistoryAsync("BTC", 
                intervalStart, intervalEnd, token);

            var ordersProfitHistory = await GetProfitHistoryAsync(idUser, start, end, token);

            var profitToBtcHistory = btcPriceHistory.Select(h =>
            {
                var profitForRequestedDate = ordersProfitHistory.FirstOrDefault(p =>
                    p.Date.Date == h.Date.Date);

                if (profitForRequestedDate != default)
                    h.Profit = profitForRequestedDate.Profit;

                return h;
            });
            
            var resultDto = await GetOrdersSummaryAsync(idUser, start, end, token);

            resultDto.CurrentBtcPrice = btcPriceHistory.LastOrDefault()?.BtcPrice ?? 0;
            resultDto.IsBtcPriceTrendUp = true; // TODO: Реализовать расчет "Методом наименьших квадратов"
            resultDto.TotalProfit = ordersProfitHistory.Sum(o => o.Profit);
            resultDto.Data = profitToBtcHistory;

            return resultDto;
        }

        public async Task<TradeTypesStatsDto> GetTradeTypesStatsAsync(int idUser, DateTime intervalStart,
            DateTime intervalEnd, CancellationToken token)
        {
            var tradeTypesInfo = await (from o in _db.Orders
                                 where o.IdUser == idUser &&
                                       o.DateClosed != null &&
                                       o.DateClosed > intervalStart &&
                                       o.DateClosed < intervalEnd
                                 group o by o.IdUser into g
                                 select new TradeTypesStatsDto
                                 {
                                     // Auto trade by third-party signals is not realized yet, but will be created in future
                                     SignalOrdersRate = 0,
                                     StopOrdersRate = Math.Round((double)g.Where(o => o.IdCreationType == 1)
                                         .Count() / g.Count()) * 100,
                                     ManualOrdersRate = Math.Round((double)g.Where(o => o.IdCreationType == 2)
                                         .Count() / g.Count()) * 100,
                                     SignalsProfit = 0,
                                     StopOrdersProfit = Math.Round(g.Where(o => o.IdCreationType == 1)
                                         .Select(o => o.Quantity * o.Price).Sum()),
                                     ManualOrdersProfit = Math.Round(g.Where(o => o.IdCreationType == 2)
                                         .Select(o => o.Quantity * o.Price).Sum()),
                                 }).FirstOrDefaultAsync(token);
            
            return tradeTypesInfo;
        }

        public async Task<IEnumerable<ProfitDetailsDto>> GetProfitDetailsAsync(int idUser, 
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token)
        {
            var profitDetailsInfo = await (from o in _db.Orders
                where o.IdUser == idUser &&
                      o.DateClosed != null &&
                      o.DateClosed > intervalStart &&
                      o.DateClosed < intervalEnd
                group o by o.DateClosed.Value.Date into g
                select new ProfitDetailsDto
                {
                    Date = g.Key,
                    // Auto trade by third-party signals is not realized yet, but will be created in future
                    SignalOrdersProfit = 0,
                    StopOrdersProfit = Math.Round(g.Where(o => o.IdCreationType == 1 && o.IdOrderStatus != 1)
                        .Select(o => o.Quantity * o.Price).Sum()),
                    ManualOrdersProfit = Math.Round(g.Where(o => o.IdCreationType == 2 && o.IdOrderStatus != 1)
                        .Select(o => o.Quantity * o.Price).Sum()),
                }).ToListAsync(token);

            return profitDetailsInfo;
        }

        private async Task<IEnumerable<(DateTime Date, double Profit)>> GetProfitHistoryAsync(int idUser, 
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token)
        {
            var ordersProfitInfo = await (from o in _db.Orders
                                where o.IdUser == idUser &&
                                      o.DateClosed != null &&
                                      o.DateClosed > intervalStart &&
                                      o.DateClosed < intervalEnd
                                group o by o.DateClosed.Value.Date into g
                                select new
                                {
                                    Date = g.Key,
                                    Spent = g.Where(o => o.IdSide == 1 && o.IdOrderStatus != 1)
                                        .Select(o => o.Quantity * o.Price).Sum(),
                                    Received = g.Where(o => o.IdSide == 2 && o.IdOrderStatus != 1)
                                        .Select(o => o.Quantity * o.Price).Sum(),
                                }).ToListAsync(token);

            var result = ordersProfitInfo.Select(o => 
                (o.Date, o.Received - o.Spent));
            return result;
        }

        private async Task<ProfitToBtcDto> GetOrdersSummaryAsync(int idUser, DateTime intervalStart, 
            DateTime intervalEnd, CancellationToken token)
        {
            var orders = await (from o in _db.Orders
                        where o.IdUser == idUser &&
                              o.DateClosed != null &&
                              o.DateClosed > intervalStart &&
                              o.DateClosed < intervalEnd
                        select o)
                        .ToListAsync(token);
            
            var result = new ProfitToBtcDto();

            if (orders is null)
                return result;

            result.TotalOrdersOpened = orders.Count();
            result.TotalOrdersClosed = orders.Count(o => o.DateClosed != null);
            result.TotalOrdersCancelled = orders.Count(o => o.IdOrderStatus == 2);
            result.AverageOrderLifeTimeMinutes = orders.Select(o =>
                (o.DateClosed - o.DateCreated).Value.TotalMinutes).Sum();

            return result;
        }
    }
}