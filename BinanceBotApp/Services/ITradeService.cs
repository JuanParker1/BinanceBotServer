using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ITradeService
    {
        Task<OrderInfoResultDto> CreateOrderAsync(OrderParamsDto orderParams,
            CancellationToken token = default);
    }
}