using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.Services;
using BinanceBotApp.Services.BackgroundWorkers;
using BinanceBotDb.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BinanceBotInfrastructure.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;
        private readonly IRefreshOrderBackgroundQueue _ordersQueue;
        private readonly IConfiguration _configuration;

        public OrdersService(IBinanceBotDbContext db, ISettingsService settingsService, 
            IHttpClientService httpService, IRefreshOrderBackgroundQueue ordersQueue,
            IConfiguration configuration)
        {
            _db = db;
            _settingsService = settingsService;
            _httpService = httpService;
            _ordersQueue = ordersQueue;
            _configuration = configuration;
        }

        public async Task<OrderInfo> GetOrderAsync(int idUser, int idOrder, // TODO: Многие методы зачем?
            string symbol, int recvWindow, CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);
            
            var uri = TradeEndpoints.GetOrderEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"orderId", idOrder == default ? null : $"{idOrder}" },
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var orderInfo = await _httpService.ProcessRequestAsync<OrderInfo>(uri, 
                qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderInfo>> GetOrdersForPairAsync(int idUser, 
            string symbol, int recvWindow, CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);
            
            var uri = TradeEndpoints.GetOpenOrdersEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var orderInfo = await _httpService.ProcessRequestAsync<IEnumerable<OrderInfo>>(uri, 
                    qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderDto>> GetActiveOrdersAsync(int idUser, int recvWindow,
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);
            
            var uri = TradeEndpoints.GetOpenOrdersEndpoint();
            var qParams = new Dictionary<string, string>
            {
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var exchangeOrdersInfo = await _httpService.ProcessRequestAsync<IEnumerable<OrderInfo>>(uri, 
                qParams, keys, HttpMethods.SignedGet, token);
            
            var dbOrdersInfo = await (from order in _db.Orders.Include(o => o.OrderType)
                where order.IdUser == idUser &&
                      order.DateClosed == null
                select order).ToListAsync(token);

            return MixOrdersInfo(exchangeOrdersInfo, dbOrdersInfo, idUser);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersHistoryForPairAsync(int idUser,
            string symbol, int days, CancellationToken token)
        {
            var startDate = DateTime.MinValue;
            
            if (days > 0)
                startDate = DateTime.Now.AddDays(-days);

            var orders = await (from order in _db.Orders.Include(o => o.OrderType)
                            where order.IdUser == idUser &&
                                  order.Symbol.StartsWith(symbol) &&
                                  order.DateCreated > startDate
                            select order).ToListAsync(token);
            
            // Additionally check that if there are no orders for that period, maybe there is unclosed order before
            // selected period and it is still alive.
            if (!orders.Any())
            {
                var lastOrderDto = await GetLastOrderAsync(idUser, symbol, token);
                if (lastOrderDto is not null && lastOrderDto.DateClosed is null)
                    return new List<OrderDto> {lastOrderDto};

                return null;
            }

            var orderDtos = orders.Select(Convert);

            return orderDtos;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersHistoryAsync(int idUser, DateTime intervalStart,
            DateTime intervalEnd, CancellationToken token)
        {
            var start = DateTime.MinValue;
            var end = DateTime.Now;

            if (intervalStart != default)
                start = intervalStart;

            if (intervalEnd != default)
                end = intervalEnd;
            
            var orders = await (from order in _db.Orders.Include(o => o.OrderType)
                            where order.IdUser == idUser &&
                                  order.DateCreated >= start &&
                                  order.DateCreated <= end
                            orderby order.Id
                            select order).ToListAsync(token);

            var dtos = orders.Select(Convert);
            return dtos;
        }

        public async Task<CreatedOrderResult> CreateTestOrderAsync(NewOrderDto newOrderDto, 
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(newOrderDto.IdUser,
                token);

            var uri = TradeEndpoints.GetTestNewOrderEndpoint();
            
            FormatOrderDtoFields(newOrderDto);
            
            var paramsToRemove = new List<string> { "idUser", "idCreationType" };

            var newOrderInfo = await _httpService.ProcessRequestAsync<NewOrderDto, CreatedOrderFull>(uri, 
                newOrderDto, keys, paramsToRemove, HttpMethods.SignedPost, token);

            return newOrderInfo;
        }
        
        public async Task CreateOrderAsync(NewOrderDto newOrderDto, CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(newOrderDto.IdUser,
                token);

            var uri = TradeEndpoints.GetOrderEndpoint();
            
            FormatOrderDtoFields(newOrderDto);
            
            _ordersQueue.EnqueueTask(async (token) =>
            {
                var paramsToRemove = new List<string> { "idUser", "idCreationType" };

                var newOrderInfo = await _httpService.ProcessRequestAsync<NewOrderDto, CreatedOrderFull>(uri, 
                    newOrderDto, keys, paramsToRemove, HttpMethods.SignedPost, token);

                var isOrderCreated = newOrderInfo.Status == "NEW" &&
                                     (!string.IsNullOrEmpty(newOrderInfo.ClientOrderId) || newOrderInfo.OrderId > 0);
          
                if (isOrderCreated)
                    await SaveOrderToDbAsync(newOrderDto, newOrderInfo, token);
            });
        }

        public async Task<DeletedOrder> DeleteOrderAsync(int idUser, long idOrder,
            string clientOrderId, string symbol, int recvWindow, CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);
            
            var uri = TradeEndpoints.GetOrderEndpoint();

            var orderId = idOrder != default 
                ? $"{idOrder}" 
                : clientOrderId;
            
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"orderId", orderId },
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            var deletedOrderInfo = await _httpService.ProcessRequestAsync<DeletedOrder>(uri, 
                    qParams, keys, HttpMethods.SignedDelete, token);

            return deletedOrderInfo;
        }
        
        public async Task<IEnumerable<DeletedOrder>> DeleteAllOrdersForPairAsync(int idUser, 
            string symbol, int recvWindow, CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);
            
            var uri = TradeEndpoints.GetOpenOrdersEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            var deletedOrdersInfo = await _httpService.ProcessRequestAsync<IEnumerable<DeletedOrder>>(uri,
                    qParams, keys, HttpMethods.Delete, token); 

            return deletedOrdersInfo;
        }

        private async Task<OrderDto> GetLastOrderAsync(int idUser, string symbol,
            CancellationToken token)
        {
            var lastOrder = await (from order in _db.Orders.Include(o => o.OrderType)
                                    where order.IdUser == idUser &&
                                          order.Symbol.StartsWith(symbol)
                                    orderby order.DateCreated
                                    select order).LastOrDefaultAsync(token);

            return lastOrder is null ? null : Convert(lastOrder);
        }

        private static IEnumerable<OrderDto> MixOrdersInfo(IEnumerable<OrderInfo> exchangeOrdersInfo,
            IEnumerable<Order> dbOrdersInfo, int idUser)
        {
            return exchangeOrdersInfo.Select(exchangeOrderInfo =>
            {
                var dbOrderInfo = dbOrdersInfo.FirstOrDefault(o => o.Symbol == exchangeOrderInfo.Symbol);
                
                return dbOrderInfo is null 
                    ? null 
                    : new OrderDto
                    {
                        Id = dbOrderInfo.Id,
                        IdUser = idUser,
                        OrderId = exchangeOrderInfo.OrderId,
                        ClientOrderId = exchangeOrderInfo.ClientOrderId,
                        Symbol = exchangeOrderInfo.Symbol,
                        Side = dbOrderInfo.IdSide == 1 ? "Покупка" : "Продажа",
                        Type = dbOrderInfo.OrderType.Caption,
                        Status = exchangeOrderInfo.Status,
                        TimeInForce = exchangeOrderInfo.TimeInForce,
                        Quantity = double.TryParse(exchangeOrderInfo.OrigQty, out var q) ? q : 0.0,
                        QuoteOrderQty = exchangeOrderInfo.OrigQuoteOrderQty,
                        DateCreated = dbOrderInfo.DateCreated,
                        DateClosed = dbOrderInfo.DateClosed,
                        CreationType = dbOrderInfo.IdCreationType == 1 ? "Авто" : "Вручную",
                        Price = double.TryParse(exchangeOrderInfo.Price, out var p) ? p : 0.0
                    };
            });
        }

        private async Task<int> SaveOrderToDbAsync(NewOrderDto newOrderDto,
            CreatedOrderFull newOrderInfo, CancellationToken token)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var context = DependencyInjection.MakeContext(connectionString);
            var orderEntity = newOrderDto.Adapt<Order>();
            orderEntity.Id = 0;
            orderEntity.DateCreated = DateTime.Now;
            orderEntity.ClientOrderId = newOrderInfo.ClientOrderId;
            orderEntity.OrderId = newOrderInfo.OrderId;
            orderEntity.IdSide = newOrderDto.Side.ToLower() == "buy"
                ? 1
                : 2;
            orderEntity.IdType = newOrderDto.Type.ToLower() == "limit"
                ? 1
                : 2;
            context.Orders.Add(orderEntity);
            return await context.SaveChangesAsync(token);
        }

        private static void FormatOrderDtoFields(NewOrderDto newOrderDto)
        {
            newOrderDto.NewOrderRespType = "FULL";
            newOrderDto.Side = newOrderDto.Side.ToUpper();
            newOrderDto.Type = newOrderDto.Type.ToUpper();
        }

        private static OrderDto Convert(Order order)
        {
            var dto = order.Adapt<OrderDto>();
            dto.Side = order.IdSide == 1 
                ? "Покупка" 
                : "Продажа";
            dto.Type = order.OrderType.Caption;
            dto.CreationType = order.IdCreationType == 1 
                ? "Авто" 
                : "Вручную";
            return dto;
        }
    }
}