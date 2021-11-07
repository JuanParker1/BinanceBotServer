using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IOrdersService
    {
        Task<OrderInfoDto> GetOrderAsync(int idOrder, string symbol, int recvWindow,
            CancellationToken token);
        Task<IEnumerable<OrderInfoDto>> GetOrdersForPairAsync(string symbol,
            int recvWindow, CancellationToken token);
        Task<IEnumerable<OrderInfoDto>> GetAllOrdersAsync(int recvWindow,
            CancellationToken token);
        Task<CreatedOrderResultDto> CreateTestOrderAsync(CreateOrderDto createOrder,
            CancellationToken token);
        Task<CreatedOrderResultDto> CreateOrderAsync(CreateOrderDto createOrder,
            CancellationToken token);
        Task<DeletedOrderDto> DeleteOrderAsync(int idOrder, string symbol, 
            int recvWindow, CancellationToken token);
        Task<IEnumerable<DeletedOrderDto>> DeleteAllOrdersForPairAsync(string symbol, 
            int recvWindow, CancellationToken token);
    }
}