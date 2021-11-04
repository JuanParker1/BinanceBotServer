using System;
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
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetOrderEndpoint();
            var qParams = ConvertToDictionary(orderParams);
            using var newOrderResponse = await _client.SignedPostRequestAsync(uri, 
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
                { "timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}" }
            };
            
            if(qParams.Symbol is not null)
                dict.Add("symbol", qParams.Symbol.ToUpper());
            
            if(qParams.Side is not null)
                dict.Add("side", qParams.Side);
            
            if(qParams.Type is not null)
                dict.Add("type", qParams.Type);
            
            if(qParams.TimeInForce is not null)
                dict.Add("timeInForce", qParams.TimeInForce);
            
            if(qParams.Quantity != default)
                dict.Add("quantity", $"{qParams.Quantity}");
            
            if(qParams.QuoteOrderQty != default)
                dict.Add("quoteOrderQty", $"{qParams.QuoteOrderQty}");
            
            if(qParams.Price != default)
                dict.Add("price", $"{qParams.Price}");
            
            if(qParams.NewClientOrderId is not null)
                dict.Add("newClientOrderID", qParams.NewClientOrderId);
            
            if(qParams.StopPrice != default)
                dict.Add("stopPrice", $"{qParams.StopPrice}");
            
            if(qParams.IcebergQty != default)
                dict.Add("icebergQty", $"{qParams.IcebergQty}");
            
            if(qParams.NewOrderRespType is not null)
                dict.Add("newOrderRespType", qParams.NewOrderRespType);
            
            if(qParams.RecvWindow != default)
                dict.Add("recvWindow", $"{qParams.RecvWindow}");
            
            return dict;
        }
    }
}