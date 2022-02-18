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

        public async Task<OrderInfoDtoOld> GetOrderAsync(int idUser, int idOrder, string symbol, 
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
            
            var orderInfo = await _httpService.ProcessRequestAsync<OrderInfoDtoOld>(uri, 
                qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderInfoDtoOld>> GetOrdersForPairAsync(int idUser, string symbol,
            int recvWindow, CancellationToken token)
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
            string symbol, DateTime intervalStart, DateTime intervalEnd, CancellationToken token)
        {
            var start = DateTime.MinValue;
            var end = DateTime.Now;

            if (intervalStart != default)
                start = intervalStart;

            if (intervalEnd != default)
                end = intervalEnd;

            var orders = await (from order in _db.Orders.Include(o => o.OrderType)
                                where order.IdUser == idUser &&
                                      order.Symbol.Contains(symbol) &&
                                      order.Date >= start &&
                                      order.Date <= end
                                select order).ToListAsync(token);

            var orderDtos = orders.Select(o =>
            {
                var dto = o.Adapt<OrderDto>();
                dto.Side = o.IdSide == 1 ? "Покупка" : "Продажа";
                dto.Type = o.OrderType.Caption;
                dto.CreationType = o.IdCreationType == 1 ? "Авто" : "Вручную";
                dto.TimeInForce = o.IdTimeInForce == 1 ? "Полный" : "Частичный";
                return dto;
            });

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
    }
}