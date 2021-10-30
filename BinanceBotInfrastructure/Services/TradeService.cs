using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceAPI.Endpoints;
using BinanceAPI.Clients.Interfaces;
using BinanceBotApp.Data;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    public class TradeService : ITradeService
    {
        private readonly IBinanceHttpClient _client;
        private readonly IHttpResponseService _responseService;

        public TradeService(IBinanceHttpClient client, IHttpResponseService responseService)
        {
            _client = client;
            _responseService = responseService;
        }

        public async Task<OrderInfoResultDto> CreateOrderAsync(OrderParamsDto orderParams,
            CancellationToken token = default)
        {
            var uri = TradeEndpoints.GetOrderEndpoint();
            var qParams = ConvertToDictionary(orderParams);
            var newOrderResponse = await _client.PostRequestAsync(uri, 
                qParams, token);
            
            var newOrderInfo = await _responseService.HandleResponseAsync<OrderInfoResultDto>(newOrderResponse, 
                token);

            return newOrderInfo;
        }

        // public async Task DeleteOrderAsync()
        // {
        //     
        // }

        private IDictionary<string, string> ConvertToDictionary(OrderParamsDto qParams)
        {
            var dict = new Dictionary<string, string>()
            {
                { "symbol", qParams.Symbol },
                { "side", $"{qParams.Side}" },
                { "type", $"{qParams.Type}" },
                { "timeInForce", $"{qParams.TimeInForce}" },
                { "quantity", $"{qParams.Quantity}" },
                { "quoteOrderQty", $"{qParams.QuoteOrderQty}" },
                { "price", $"{qParams.Price}" },
                { "newClientOrderID", $"{qParams.NewClientOrderId}" },
                { "stopPrice", $"{qParams.StopPrice}" },
                { "icebergQty", $"{qParams.IcebergQty}" },
                { "newOrderRespType", $"{qParams.NewOrderRespType}" },
                { "recvWindow", $"{qParams.RecvWindow}" },
                { "timestamp", $"{qParams.Timestamp}" }
            };

            return dict;
        }
    }
}