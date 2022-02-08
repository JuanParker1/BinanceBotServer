using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IOrdersService
    {
        Task<OrderInfoDto> GetOrderAsync(int idUser, int idOrder, 
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<OrderInfoDto>> GetOrdersForPairAsync(int idUser,
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<OrderInfoDto>> GetAllOrdersAsync(int idUser, 
            int recvWindow, CancellationToken token);
        Task<CreatedOrderResultDto> CreateTestOrderAsync(CreateOrderDto createOrderDto, 
            CancellationToken token);
        Task<CreatedOrderResultDto> CreateOrderAsync(CreateOrderDto createOrderDto, 
            CancellationToken token);
        Task<DeletedOrderDto> DeleteOrderAsync(int idUser, int idOrder, 
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<DeletedOrderDto>> DeleteAllOrdersForPairAsync(int idUser, 
            string symbol, int recvWindow, CancellationToken token);
    }
}