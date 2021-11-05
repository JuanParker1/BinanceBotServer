using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ITradeService
    {
        Task<OrderInfoResultDto> CreateTestOrderAsync(OrderParamsDto orderParams,
            CancellationToken token);
        Task<OrderInfoResultDto> CreateOrderAsync(OrderParamsDto orderParams,
            CancellationToken token);
        Task<DeletedOrderInfoDto> DeleteOrderAsync(int idOrder, string symbol, 
            int recvWindow = 5000, CancellationToken token = default);
        Task<IEnumerable<DeletedOrderInfoDto>> DeleteAllOrdersForPairAsync(string symbol, 
            int recvWindow = 5000, CancellationToken token = default);
    }
}