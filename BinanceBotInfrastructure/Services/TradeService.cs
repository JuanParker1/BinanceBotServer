using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services
{
    public class TradeService : ITradeService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;

        public TradeService(IBinanceBotDbContext db, ISettingsService settingsService, 
            IHttpClientService httpService)
        {
            _db = db;
            _settingsService = settingsService;
            _httpService = httpService;
        }

        public async Task<OrderInfoDtoOld> GetOrderAsync(int idUser, int idOrder, 
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
            
            var orderInfo = await _httpService.ProcessRequestAsync<OrderInfoDtoOld>(uri, 
                qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderInfoDtoOld>> GetOrdersForPairAsync(int idUser, 
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
            
            var orderInfo = await _httpService.ProcessRequestAsync<IEnumerable<OrderInfoDtoOld>>(uri, 
                    qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
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
        
        public async Task<IEnumerable<OrderInfoDtoOld>> GetAllOrdersAsync(int idUser, int recvWindow,
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(idUser,
                token);
            
            var uri = TradeEndpoints.GetOpenOrdersEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var orderInfo = await _httpService.ProcessRequestAsync<IEnumerable<OrderInfoDtoOld>>(uri, 
                qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
        }
        
        public async Task<CreatedOrderResultDto> CreateTestOrderAsync(NewOrderDto newOrderDto, 
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(newOrderDto.IdUser,
                token);

            var uri = TradeEndpoints.GetTestNewOrderEndpoint();

            var newOrderInfo = await _httpService.ProcessRequestAsync<NewOrderDto, CreatedOrderResultDto>(uri, 
                newOrderDto, keys, HttpMethods.SignedPost, token);

            return newOrderInfo;
        }
        
        public async Task<CreatedOrderResultDto> CreateOrderAsync(NewOrderDto newOrderDto, 
            CancellationToken token)
        {
            var keys = await _settingsService.GetApiKeysAsync(newOrderDto.IdUser,
                token);

            var uri = TradeEndpoints.GetOrderEndpoint();

            var newOrderInfo = await _httpService.ProcessRequestAsync<NewOrderDto, CreatedOrderResultDto>(uri, 
                newOrderDto, keys, HttpMethods.SignedPost, token);

            return newOrderInfo;
        }

        public async Task<DeletedOrderDto> DeleteOrderAsync(int idUser, int idOrder, string symbol, 
            int recvWindow, CancellationToken token)
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
            var deletedOrderInfo = await _httpService.ProcessRequestAsync<DeletedOrderDto>(uri, 
                    qParams, keys, HttpMethods.SignedDelete, token);

            return deletedOrderInfo;
        }
        
        public async Task<IEnumerable<DeletedOrderDto>> DeleteAllOrdersForPairAsync(int idUser, 
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
            var deletedOrdersInfo = await _httpService.ProcessRequestAsync<IEnumerable<DeletedOrderDto>>(uri,
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

        private static OrderDto Convert(Order order)
        {
            var dto = order.Adapt<OrderDto>();
            dto.Side = order.IdSide == 1 ? "Покупка" : "Продажа";
            dto.Type = order.OrderType.Caption;
            dto.CreationType = order.IdCreationType == 1 ? "Авто" : "Вручную";
            dto.TimeInForce = order.IdTimeInForce == 1 ? "Полный" : "Частичный";
            return dto;
        }
    }
}