using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    /// <summary>
    /// Http client for Binance api.
    /// </summary>
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private const string _apiKeyHeader = "X-MBX-APIKEY";
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private string _secretKey;
        public string SecretKey
        {
            set => _secretKey = value;
        }
        
        private string _apiKey;
        public string ApiKey
        {
            set
            {
                _apiKey = value;
                
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
                PropertyNameCaseInsensitive = true
            };
        }
        
        public async Task<TResult> ProcessRequestAsync<TDto, TResult>(Uri uri, // TODO: Такой вариант точно нужен? Или хватит только который со словарем ниже?
            TDto dto, (string apiKey, string secretKey) keys, HttpMethods requestType, 
            CancellationToken token) where TResult : class
        {
            _apiKey = keys.apiKey;
            _secretKey = keys.secretKey;
            
            var qParams = ConvertToDictionary(dto);

            var responseInfo = await ProcessRequestAsync<TResult>(uri, 
                qParams, keys, requestType, token);

            return responseInfo;
        }
        
        public async Task<TResult> ProcessRequestAsync<TResult>(Uri uri, 
            IDictionary<string, string> qParams, (string apiKey, string secretKey) keys, 
            HttpMethods requestType, CancellationToken token) 
            where TResult : class
        {
            _apiKey = keys.apiKey;
            _secretKey = keys.secretKey;
            
            Func<Uri, IDictionary<string, string>, CancellationToken, 
                Task<HttpResponseMessage>> requestDelegate =
                    requestType switch
                    {
                        HttpMethods.Get => GetRequestAsync,
                        HttpMethods.SignedGet => SignedGetRequestAsync,
                        HttpMethods.Post => PostRequestAsync,
                        HttpMethods.SignedPost => SignedPostRequestAsync, // TODO: Очень много однотипных методов с разными запросами. Через делегат упростить?
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

                if (value != default && IsNumericDefault(value))
                    continue;

                if(value != default)
                    resultDict.Add(camelCasedKey, $"{value}");
            }
            
            return resultDict;
        }

        private static bool IsNumericDefault(object value)  // TODO: Убрать в Инфрастракче.Сервсез.Утилз. И конвертацию в словарь выше тоже.
        {
            var intVal = 1;
            var doubleVal = 1.0;

            return (int.TryParse($"{value}", out intVal) || 
                    double.TryParse($"{value}", out doubleVal)) && 
                   (intVal == default || doubleVal == default);
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
            var uri = CreateValidUri(endpoint, qParams);
            return _httpClient.GetAsync(uri, token);   
        }
        
        private Task<HttpResponseMessage> SignedGetRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedUri = CreateValidSignedUri(endpoint, qParams);
            return _httpClient.GetAsync(signedUri, token);
        }
        
        private Task<HttpResponseMessage> PostRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var queryParams = MakeUrlEncodedContent(qParams);
            return _httpClient.PostAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> SignedPostRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedParams = AddSignatureToDictionary(qParams);
            var queryParams = MakeUrlEncodedContent(signedParams);
            
            return _httpClient.PostAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> PutRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var queryParams = MakeUrlEncodedContent(qParams);
            return _httpClient.PutAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> SignedPutRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedParams = AddSignatureToDictionary(qParams);
            var queryParams = MakeUrlEncodedContent(signedParams);
            
            return _httpClient.PutAsync(endpoint, queryParams, token);
        }
        
        private Task<HttpResponseMessage> DeleteRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, CancellationToken token = default)
        {
            var uri = CreateValidUri(endpoint, qParams);
            return _httpClient.DeleteAsync(uri, token);   
        }
        
        private Task<HttpResponseMessage> SignedDeleteRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedUri = CreateValidSignedUri(endpoint, qParams);
            return _httpClient.DeleteAsync(signedUri, token);
        }

        private static FormUrlEncodedContent MakeUrlEncodedContent(
            IDictionary<string, string> qParams = default)
        {
            qParams ??= new Dictionary<string, string>();
            
            var content = new FormUrlEncodedContent(qParams);
            return content;
        }
                
        private static Uri CreateValidUri(Uri endpoint, IDictionary<string, string> qParams = default)
        {
            var queryString = ConvertToParamsString(qParams);
            var resultQueryString = $"{endpoint}";
            
            if(queryString != string.Empty)
                resultQueryString = $"{endpoint}?{queryString}";
            
            var uri = new Uri(resultQueryString);
            return uri;
        }

        private Uri CreateValidSignedUri(Uri endpoint, IDictionary<string, string> qParams = default)
        {
            if (qParams == default)
                return endpoint;
            
            var queryParamsString = ConvertToParamsString(qParams);
            var hmacResult = CreateHmacSignature(_secretKey, queryParamsString);
            
            var resultUri = new Uri($"{endpoint.AbsoluteUri}?" +
                                    $"{queryParamsString}&signature={hmacResult}");
            return resultUri;
        }

        private IDictionary<string, string> AddSignatureToDictionary(
            IDictionary<string, string> qParams = default)
        {
            if (qParams == default)
                return qParams;

            var queryParamsString = ConvertToParamsString(qParams);
            var signature = CreateHmacSignature(_secretKey, queryParamsString);   
            qParams["signature"] = signature;

            return qParams;
        }

        private static string ConvertToParamsString(IDictionary<string, string> qParams = default)
        {
            var queryString = string.Empty;

            if (qParams == default || !qParams.Any()) 
                return queryString;
            
            queryString = string.Join("&", qParams.Select(kvp =>
                    $"{kvp.Key}={kvp.Value}"));

            return queryString;
        }
        
        private static string CreateHmacSignature(string secretKey, string totalParams)
        {
            var messageBytes = Encoding.UTF8.GetBytes(totalParams);
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var hash = new HMACSHA256(keyBytes);
            var computedHash = hash.ComputeHash(messageBytes);
            
            return BitConverter.ToString(computedHash)
                .Replace("-", "").ToLower();
        }
    }
}