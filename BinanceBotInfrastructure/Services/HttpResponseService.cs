using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Enums;
using BinanceAPI.Clients.Interfaces;

namespace BinanceBotInfrastructure.Services
{
    public class HttpResponseService : IHttpResponseService
    {
        private readonly IBinanceHttpClient _client;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public HttpResponseService(IBinanceHttpClient client)
        {
            _client = client;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
        
        public async Task<TResult> ProcessRequestAsync<TDto, TResult>(Uri uri, TDto dto, HttpMethods requestType,  
            CancellationToken token) where TResult : class
        {
            var qParams = ConvertToDictionary(dto);

            var responseInfo = await ProcessRequestAsync<TResult>(uri, qParams, 
                requestType, token);

            return responseInfo;
        }

        public async Task<TResult> ProcessRequestAsync<TResult>(Uri uri, IDictionary<string, string> qParams,
            HttpMethods requestType, CancellationToken token) where TResult : class
        {
            TResult responseInfo;
            
            switch (requestType)
            {
                case HttpMethods.Get:
                    using (var newOrderResponse = await _client.GetRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await HandleResponseAsync<TResult>(newOrderResponse, 
                            token);
                        break;
                    }
                case HttpMethods.SignedGet:
                    using (var newOrderResponse = await _client.SignedGetRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await HandleResponseAsync<TResult>(newOrderResponse, 
                            token);
                        break;
                    }
                case HttpMethods.Post:
                    using (var response = await _client.PostRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await HandleResponseAsync<TResult>(response, 
                            token);   
                        break;
                    }
                case HttpMethods.SignedPost:
                    using (var newOrderResponse = await _client.SignedPostRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.Put:
                    using (var newOrderResponse = await _client.PutRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.SignedPut:
                    using (var newOrderResponse = await _client.SignedPutRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.Delete:
                    using (var newOrderResponse = await _client.DeleteRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                case HttpMethods.SignedDelete:
                    using (var newOrderResponse = await _client.SignedDeleteRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = await HandleResponseAsync<TResult>(newOrderResponse, 
                            token);   
                        break;
                    }
                default:
                    using (var newOrderResponse = await _client.GetRequestAsync(uri,
                        qParams, token))
                    {
                        responseInfo = 
                            await HandleResponseAsync<TResult>(newOrderResponse, 
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
        
        private async Task<TResult> HandleResponseAsync<TResult>(HttpResponseMessage message, 
            CancellationToken token) where TResult : class
        {
            if (message is null) 
                throw new ArgumentNullException("Message is null");
            
            var messageJson = await message.Content.ReadAsStringAsync(token)
                .ConfigureAwait(false);
            
            if (!message.IsSuccessStatusCode)
            {
                var errorObj = JsonSerializer.Deserialize<ApiErrorDto>(messageJson, _jsonSerializerOptions) 
                               ?? new ApiErrorDto();

                var errorMessage = $"Http status code: {(int)message.StatusCode} \n" +
                                   $"Binance error code: {errorObj.Code} \n" +
                                   $"Binance error message: {errorObj.Msg}";
                
                throw new InvalidOperationException(errorMessage);
            }

            var resultDto = JsonSerializer.Deserialize<TResult>(messageJson, _jsonSerializerOptions);

            return resultDto;
        }
    }
}