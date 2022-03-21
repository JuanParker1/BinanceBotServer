using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Deserializers;

namespace BinanceBotApp.Services
{
    public interface IOrdersService
    {
        Task<OrderInfo> GetOrderAsync(int idUser, int idOrder, 
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<OrderInfo>> GetOrdersForPairAsync(int idUser,
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<OrderDto>> GetOrdersHistoryForPairAsync(int idUser,
            string symbol, int days, CancellationToken token);
        Task<IEnumerable<OrderDto>> GetOrdersHistoryAsync(int idUser, DateTime intervalStart,
            DateTime intervalEnd, CancellationToken token);
        Task<IEnumerable<OrderDto>> GetActiveOrdersAsync(int idUser, 
            int recvWindow, CancellationToken token);
        Task<CreatedOrderResult> CreateTestOrderAsync(NewOrderDto newOrderDto, 
            CancellationToken token);
        Task CreateOrderAsync(NewOrderDto newOrderDto, CancellationToken token);
        Task<DeletedOrder> DeleteOrderAsync(int idUser, int idOrder, 
            string symbol, int recvWindow, CancellationToken token);
        Task<IEnumerable<DeletedOrder>> DeleteAllOrdersForPairAsync(int idUser, 
            string symbol, int recvWindow, CancellationToken token);
    }
}