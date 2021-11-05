using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceAPI.Endpoints;
using BinanceAPI.Clients.Interfaces;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Enums;
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

        public async Task<OrderInfoResultDto> CreateTestOrderAsync(OrderParamsDto orderParams,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetTestNewOrderEndpoint();

            var newOrderInfo = await ProcessRequest<OrderParamsDto, OrderInfoResultDto>(uri, 
                orderParams, HttpMethods.SignedPost, token);

            return newOrderInfo;
        }
        
        public async Task<OrderInfoResultDto> CreateOrderAsync(OrderParamsDto orderParams,
            CancellationToken token)
        {
            var uri = TradeEndpoints.GetNewOrderEndpoint();

            var newOrderInfo = await ProcessRequest<OrderParamsDto, OrderInfoResultDto>(uri, 
                orderParams, HttpMethods.SignedPost, token);

            return newOrderInfo;
        }

        public async Task<DeletedOrderInfoDto> DeleteOrderAsync(int idOrder, string symbol, 
            int recvWindow = 5000, CancellationToken token = default)
        {
            var uri = TradeEndpoints.GetNewOrderEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol.ToUpper()},
                {"orderId", $"{idOrder}"},
                {"recvWindow", $"{recvWindow}"},
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            using var newOrderResponse = await _client.SignedDeleteRequestAsync(uri, 
                qParams, token);
            
            var newOrderInfo = 
                await _responseService.HandleResponseAsync<DeletedOrderInfoDto>(newOrderResponse, 
                token);
        
            return newOrderInfo;
        }
        
        public async Task<IEnumerable<DeletedOrderInfoDto>> DeleteAllOrdersForPairAsync(string symbol, 
            int recvWindow = 5000, CancellationToken token = default)
        {
            var uri = TradeEndpoints.GetOpenOrdersStatusEndpoint();
            var qParams = new Dictionary<string, string>()
            {
                {"symbol", symbol.ToUpper()},
                {"recvWindow", $"{recvWindow}"},
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            using var newOrderResponse = await _client.DeleteRequestAsync(uri, 
                qParams, token);
            
            var newOrderInfo = 
                await _responseService.HandleResponseAsync<IEnumerable<DeletedOrderInfoDto>>(newOrderResponse, 
                    token);
        
            return newOrderInfo;
        }

        private async Task<TResult> ProcessRequest<TDto, TResult>(Uri uri, TDto dto, HttpMethods requestType,  
            CancellationToken token) where TResult : class
        {
            var qParams = ConvertToDictionary(dto);

            TResult responseInfo;
            
            switch (requestType)
            {
                case HttpMethods.Get:
                    using (var newOrderResponse = await _client.GetRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                            token);
                        break;
                    }
                case HttpMethods.SignedGet:
                    using (var newOrderResponse = await _client.SignedGetRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                            token);
                        break;
                    }
                case HttpMethods.Post:
                    using (var response = await _client.PostRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await _responseService.HandleResponseAsync<TResult>(response, 
                            token);   
                        break;
                    }
                case HttpMethods.SignedPost:
                    using (var newOrderResponse = await _client.SignedPostRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.Put:
                    using (var newOrderResponse = await _client.PutRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.SignedPut:
                    using (var newOrderResponse = await _client.SignedPutRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.Delete:
                    using (var newOrderResponse = await _client.DeleteRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.SignedDelete:
                    using (var newOrderResponse = await _client.SignedDeleteRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                default:
                    using (var newOrderResponse = await _client.GetRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await _responseService.HandleResponseAsync<TResult>(newOrderResponse, 
                                token);
                        break;
                    }
            }

            return responseInfo;
        }
        
        private static IDictionary<string, string> ConvertToDictionary<T>(T dto)
        {
            var resultDict = new Dictionary<string, string>()
            {
                {"timestamp", $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}"}
            };
            
            var typeFieldsNames = dto.GetType().GetMembers()
                .Where(m => m.MemberType == MemberTypes.Property)
                .Select(f => f.Name);

            foreach (var name in typeFieldsNames)
            {
                var camelCasedKey = char.ToLower(name[0]) + name[1..];
                var value = dto.GetType()?.GetProperty(name)?.GetValue(dto);
                if(value is not null)
                    resultDict.Add(camelCasedKey, $"{value}");
            }
            
            return resultDict;
        }
    }
}