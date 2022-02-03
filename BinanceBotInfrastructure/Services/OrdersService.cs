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
    public class OrdersService : IOrdersService
    {
        private readonly IHttpClientService _httpClientService;

        public OrdersService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<OrderInfoDto> GetOrderAsync(int idOrder, string symbol, int recvWindow,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetOrderEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"orderId", idOrder == default ? null : $"{idOrder}" },
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var orderInfo = await _httpClientService.ProcessRequestAsync<OrderInfoDto>(uri, 
                qParams, HttpMethods.SignedGet, token)
                .ConfigureAwait(false);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderInfoDto>> GetOrdersForPairAsync(string symbol,
            int recvWindow, CancellationToken token)
        {
            var uri = TradeEndpoints.GetOpenOrdersEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var orderInfo = 
                await _httpClientService.ProcessRequestAsync<IEnumerable<OrderInfoDto>>(uri, 
                    qParams, HttpMethods.SignedGet, token)
                    .ConfigureAwait(false);

            return orderInfo;
        }
        
        public async Task<IEnumerable<OrderInfoDto>> GetAllOrdersAsync(int recvWindow,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetOpenOrdersEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var orderInfo = await _httpClientService.ProcessRequestAsync<IEnumerable<OrderInfoDto>>(uri, 
                qParams, HttpMethods.SignedGet, token)
                .ConfigureAwait(false);

            return orderInfo;
        }
        
        public async Task<CreatedOrderResultDto> CreateTestOrderAsync(CreateOrderDto createOrder,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetTestNewOrderEndpoint();

            var newOrderInfo = await _httpClientService.ProcessRequestAsync<CreateOrderDto, CreatedOrderResultDto>(uri, 
                createOrder, HttpMethods.SignedPost, token)
                .ConfigureAwait(false);

            return newOrderInfo;
        }
        
        public async Task<CreatedOrderResultDto> CreateOrderAsync(CreateOrderDto createOrder,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetOrderEndpoint();

            var newOrderInfo = await _httpClientService.ProcessRequestAsync<CreateOrderDto, CreatedOrderResultDto>(uri, 
                createOrder, HttpMethods.SignedPost, token)
                .ConfigureAwait(false);

            return newOrderInfo;
        }

        public async Task<DeletedOrderDto> DeleteOrderAsync(int idOrder, string symbol, 
            int recvWindow, CancellationToken token)
        {
            var uri = TradeEndpoints.GetOrderEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"orderId", idOrder == default ? null : $"{idOrder}" },
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            var deletedOrderInfo = await _httpClientService.ProcessRequestAsync<DeletedOrderDto>(uri, 
                    qParams, HttpMethods.SignedDelete, token)
                .ConfigureAwait(false);

            return deletedOrderInfo;
        }
        
        public async Task<IEnumerable<DeletedOrderDto>> DeleteAllOrdersForPairAsync(string symbol, 
            int recvWindow, CancellationToken token)
        {
            var uri = TradeEndpoints.GetOpenOrdersEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            var deletedOrdersInfo = await _httpClientService.ProcessRequestAsync<IEnumerable<DeletedOrderDto>>(uri,
                    qParams, HttpMethods.Delete, token)
                .ConfigureAwait(false); 

            return deletedOrdersInfo;
        }
    }
}