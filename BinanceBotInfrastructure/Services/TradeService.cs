using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    public class TradeService : ITradeService
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpClientService _httpService;

        public TradeService(ISettingsService settingsService, 
            IHttpClientService httpService)
        {
            _settingsService = settingsService;
            _httpService = httpService;
        }

        public async Task<OrderInfoDto> GetOrderAsync(int idUser, int idOrder, string symbol, 
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
            
            var orderInfo = await _httpService.ProcessRequestAsync<OrderInfoDto>(uri, 
                qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderInfoDto>> GetOrdersForPairAsync(int idUser, string symbol,
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
            
            var orderInfo = await _httpService.ProcessRequestAsync<IEnumerable<OrderInfoDto>>(uri, 
                    qParams, keys, HttpMethods.SignedGet, token);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderInfoDto>> GetAllOrdersAsync(int idUser, int recvWindow,
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
            
            var orderInfo = await _httpService.ProcessRequestAsync<IEnumerable<OrderInfoDto>>(uri, 
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