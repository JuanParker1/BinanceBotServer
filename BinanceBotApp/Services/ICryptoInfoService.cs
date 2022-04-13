using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data.Analytics;

namespace BinanceBotApp.Services;

public interface ICryptoInfoService
{
    Task<IEnumerable<ProfitToBtcHistoryDto>> GetPriceHistoryAsync(string symbol, 
        DateTime intervalStart, DateTime intervalEnd, CancellationToken token);
}