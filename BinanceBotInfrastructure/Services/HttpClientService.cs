using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Deserializers.Converters;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Utils;

namespace BinanceBotInfrastructure.Services
{
    /// <summary>
    /// Http client for Binance and third-party apis.
    /// </summary>
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private const string _apiKeyHeader = "X-MBX-APIKEY";
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        
        private string SecretKey { get; set; }

        private string ApiKey
        {
            set
            {
                var mt = new MediaTypeWithQualityHeaderValue("application/json");
                _httpClient.DefaultRequestHeaders.Accept.Add(mt);
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(_apiKeyHeader,
                    new[] { value });
            }
        }
        
        public HttpClientService()
        {
            _httpClient = new HttpClient();
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
            _jsonSerializerOptions.Converters.Add(new StringConverter());
        }

        public async Task<TResult> GetRequestAsync<TResult>(string url,
            CancellationToken token) where TResult : class
        {
            var response = await _httpClient.GetAsync(url, token);   
            
            var responseInfo = await HandleResponseAsync<TResult>(response, 
                token);

            return responseInfo;
        }
        
        public async Task<TResult> ProcessRequestAsync<TDto, TResult>(Uri uri,
            TDto dto, (string apiKey, string secretKey) keys, IEnumerable<string> paramsToRemove, 
            HttpMethods requestType, CancellationToken token) where TResult : class
        {
            var qParams = Converter.ToDictionary(dto, 
                paramsToRemove);

            var responseInfo = await ProcessRequestAsync<TResult>(uri, 
                qParams, keys, requestType, token);

            return responseInfo;
        }
        
        public async Task<TResult> ProcessRequestAsync<TResult>(Uri uri, 
            IDictionary<string, string> qParams, (string apiKey, string secretKey) keys, 
            HttpMethods requestType, CancellationToken token) 
            where TResult : class
        {
            ApiKey = keys.apiKey;
            SecretKey = keys.secretKey;
     
            if (string.IsNullOrEmpty(keys.apiKey) || string.IsNullOrEmpty(keys.secretKey))
                return null;
            
            Func<Uri, IDictionary<string, string>, CancellationToken, 
                Task<HttpResponseMessage>> requestDelegate =
                    requestType switch
                    {
                        HttpMethods.Get => GetRequestAsync,
                        HttpMethods.SignedGet => SignedGetRequestAsync,
                        HttpMethods.Post => PostRequestAsync,
                        HttpMethods.SignedPost => SignedPostRequestAsync,
                        HttpMethods.Put => PutRequestAsync,
                        HttpMethods.SignedPut => SignedPutRequestAsync,
                        HttpMethods.Delete => DeleteRequestAsync,
                        HttpMethods.SignedDelete => SignedDeleteRequestAsync,
                        _ => SignedGetRequestAsync
                    };

            using var newOrderResponse = await requestDelegate(uri, qParams, token);
                
            var responseInfo = await HandleResponseAsync<TResult>(newOrderResponse, 
                token);

            return responseInfo;
        }

        private async Task<TResult> HandleResponseAsync<TResult>(HttpResponseMessage message, 
            CancellationToken token) where TResult : class
        {
            if (message is null) 
                throw new ArgumentNullException("Message is null");
            
            var messageJson = await message.Content.ReadAsStringAsync(token);

            if (!message.IsSuccessStatusCode)
            {
                var errorObj = JsonSerializer.Deserialize<ApiErrorDto>(messageJson, 
                                   _jsonSerializerOptions) ?? new ApiErrorDto();

                var errorMessage = $"Http status code: {(int)message.StatusCode} \n" +
                                   $"Binance error code: {errorObj.Code} \n" +
                                   $"Binance error message: {errorObj.Msg}";
                
                Trace.TraceError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            var resultDto = JsonSerializer.Deserialize<TResult>(messageJson, 
                _jsonSerializerOptions);

            return resultDto;
        }
        
        private Task<HttpResponseMessage> GetRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var uri = Converter.ToValidUri(endpoint, qParams);
            return _httpClient.GetAsync(uri, token);   
        }
        
        private Task<HttpResponseMessage> SignedGetRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedUri = Converter.ToValidSignedUri(endpoint, SecretKey, 
                qParams);
            return _httpClient.GetAsync(signedUri, token);
        }
        
        private Task<HttpResponseMessage> PostRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var queryParams = Converter.ToUrlEncodedContent(qParams);
            return _httpClient.PostAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> SignedPostRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedParams = AddSignatureToDictionary(qParams);
            var queryParams = Converter.ToUrlEncodedContent(signedParams);
            
            return _httpClient.PostAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> PutRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var queryParams = Converter.ToUrlEncodedContent(qParams);
            return _httpClient.PutAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> SignedPutRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedParams = AddSignatureToDictionary(qParams);
            var queryParams = Converter.ToUrlEncodedContent(signedParams);
            
            return _httpClient.PutAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> DeleteRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var uri = Converter.ToValidUri(endpoint, qParams);
            return _httpClient.DeleteAsync(uri, token);   
        }
        
        private Task<HttpResponseMessage> SignedDeleteRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedUri = Converter.ToValidSignedUri(endpoint, SecretKey, 
                qParams);
            return _httpClient.DeleteAsync(signedUri, token);
        }

        private IDictionary<string, string> AddSignatureToDictionary(
            IDictionary<string, string> qParams = default)
        {
            if (qParams == default)
                return qParams;

            var queryParamsString = Converter.ToParamsString(qParams);
            var signature = Converter.ToHmacSignature(SecretKey, 
                queryParamsString);   
            qParams["signature"] = signature;

            return qParams;
        }
    }
}