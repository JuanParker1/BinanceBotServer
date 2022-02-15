using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ITradeService
    {
        Task<OrderInfoDto> GetOrderAsync(int idUser, int idOrder, 
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<OrderInfoDto>> GetOrdersForPairAsync(int idUser,
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<OrderInfoDto>> GetAllOrdersAsync(int idUser, 
            int recvWindow, CancellationToken token);
        Task<CreatedOrderResultDto> CreateTestOrderAsync(NewOrderDto newOrderDto, 
            CancellationToken token);
        Task<CreatedOrderResultDto> CreateOrderAsync(NewOrderDto newOrderDto, 
            CancellationToken token);
        Task<DeletedOrderDto> DeleteOrderAsync(int idUser, int idOrder, 
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<DeletedOrderDto>> DeleteAllOrdersForPairAsync(int idUser, 
            string symbol, int recvWindow, CancellationToken token);
    }
}