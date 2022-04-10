using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using BinanceBotApp.Data.Analytics;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly IHttpClientService _httpService;

        public AnalyticsService(IBinanceBotDbContext db,
            IHttpClientService httpService)
        {
            _db = db;
            _httpService = httpService;
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

            var btcPriceHistory = await GetBtcPriceHistoryAsync((end - start).Days, token);

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
            resultDto.ProfitToBtcHistory = profitToBtcHistory;

            return resultDto;
        }
        
        private async Task<IEnumerable<ProfitToBtcHistoryDto>> GetBtcPriceHistoryAsync(int intervalDays,
            CancellationToken token)
        {
            var btcPriceRequestUrl =
                $"https://min-api.cryptocompare.com/data/v2/histoday?fsym=BTC&tsym=USDT&limit={intervalDays}";

            var btcPriceHistory = await _httpService.GetRequestAsync<PriceApiResponse>(btcPriceRequestUrl,
                token);

            return btcPriceHistory.Data.Data.Select(h => 
                new ProfitToBtcHistoryDto
                {
                    Date = new DateTime(h.Time),
                    BtcPrice = h.High,
                    Profit = 0
                }
            );
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
                                    Spent = g.Where(o => o.IdSide == 1)
                                        .Select(o => o.Quantity * o.Price).Sum(),
                                    Received = g.Where(o => o.IdSide == 2)
                                        .Select(o => o.Quantity * o.Price).Sum(),
                                }).ToListAsync(token);

            var result = ordersProfitInfo.Select(o => 
                (o.Date, o.Received - o.Spent));
            return result;
        }

        private async Task<ProfitToBtcDto> GetOrdersSummaryAsync(int idUser, 
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token)
        {
            var ordersInfo = await (from o in _db.Orders
                            where o.IdUser == idUser &&
                            o.DateClosed > intervalStart &&
                            o.DateClosed < intervalEnd
                            group o by o.IdUser
                            into g
                            select new ProfitToBtcDto
                            {
                                TotalOrdersOpened = g.Count(),
                                TotalOrdersClosed = g.Count(o => o.DateClosed != null),
                                TotalOrdersCancelled = g.Count(o => o.IdOrderStatus == 2),
                                AverageOrderLifeTimeMinutes = g.Select(o => 
                                    (o.DateClosed - o.DateCreated).Value.Minutes).Sum() / g.Count()
                            }).FirstOrDefaultAsync(token);

            return ordersInfo;
        }
    }
}