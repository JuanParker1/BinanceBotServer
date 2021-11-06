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
        private readonly IHttpResponseService _responseService;

        public TradeService(IHttpResponseService responseService)
        {
            _responseService = responseService;
        }

        public async Task<OrderInfoResultDto> CreateTestOrderAsync(OrderParamsDto orderParams,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetTestNewOrderEndpoint();

            var newOrderInfo = 
                await _responseService.ProcessRequestAsync<OrderParamsDto, OrderInfoResultDto>(uri, 
                orderParams, HttpMethods.SignedPost, token);

            return newOrderInfo;
        }
        
        public async Task<OrderInfoResultDto> CreateOrderAsync(OrderParamsDto orderParams,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetNewOrderEndpoint();

            var newOrderInfo = 
                await _responseService.ProcessRequestAsync<OrderParamsDto, OrderInfoResultDto>(uri, 
                orderParams, HttpMethods.SignedPost, token);

            return newOrderInfo;
        }

        public async Task<DeletedOrderInfoDto> DeleteOrderAsync(int idOrder, string symbol, 
            int recvWindow, CancellationToken token)
        {
            var uri = TradeEndpoints.GetNewOrderEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"orderId", idOrder == default ? null : $"{idOrder}" },
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            var deletedOrderInfo = 
                await _responseService.ProcessRequestAsync<DeletedOrderInfoDto>(uri, 
                    qParams, HttpMethods.SignedDelete, token);

            return deletedOrderInfo;
        }
        
        public async Task<IEnumerable<DeletedOrderInfoDto>> DeleteAllOrdersForPairAsync(string symbol, 
            int recvWindow, CancellationToken token)
        {
            var uri = TradeEndpoints.GetOpenOrdersStatusEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol == default ? null : $"{symbol.ToUpper()}"},
                {"recvWindow", recvWindow == default ? null : $"{recvWindow}" },
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            var deletedOrdersInfo = 
                await _responseService.ProcessRequestAsync<IEnumerable<DeletedOrderInfoDto>>(uri,
                    qParams, HttpMethods.Delete, token); 

            return deletedOrdersInfo;
        }
    }
}