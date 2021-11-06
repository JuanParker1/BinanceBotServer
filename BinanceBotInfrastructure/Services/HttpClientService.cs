using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    /// <summary>
    /// Http client for Binance api
    /// </summary>
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private const string _apiKeyHeader = "X-MBX-APIKEY";
        private readonly string _secretKey;

        public HttpClientService(string apiKey, string secretKey)
        {
            _httpClient = new HttpClient();
            _secretKey = secretKey;
            var mt = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(mt);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(_apiKeyHeader, 
            new[] { apiKey });
        }

        /// <summary>
        /// Create a simple GET request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> GetRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var uri = CreateValidUri(endpoint, qParams);
            return _httpClient.GetAsync(uri, token);   
        }

        /// <summary>
        /// Creates a signed GET request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> SignedGetRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedUri = CreateValidSignedUri(endpoint, qParams);
            return _httpClient.GetAsync(signedUri, token);
        }

        /// <summary>
        /// Creates a simple POST request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> PostRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var queryParams = MakeUrlEncodedContent(qParams);
            return _httpClient.PostAsync(endpoint, queryParams, token);
        }

        /// <summary>
        /// Creates a signed POST request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> SignedPostRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedParams = AddSignatureToDictionary(qParams);
            var queryParams = MakeUrlEncodedContent(signedParams);
            
            return _httpClient.PostAsync(endpoint, queryParams, token);
        }

        /// <summary>
        /// Creates a simple PUT request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> PutRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var queryParams = MakeUrlEncodedContent(qParams);
            return _httpClient.PutAsync(endpoint, queryParams, token);
        }

        /// <summary>
        /// Creates a signed PUT request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> SignedPutRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams = default, 
            CancellationToken token = default)
        {
            var signedParams = AddSignatureToDictionary(qParams);
            var queryParams = MakeUrlEncodedContent(signedParams);
            
            return _httpClient.PutAsync(endpoint, queryParams, token);
        }

        /// <summary>
        /// Creates a simple DELETE request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> DeleteRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams = default, CancellationToken token = default)
        {
            var uri = CreateValidUri(endpoint, qParams);
            return _httpClient.DeleteAsync(uri, token);   
        }

        /// <summary>
        /// Creates a signed DELETE request
        /// </summary>
        /// <param name="endpoint"> Request endpoint </param>
        /// <param name="qParams"> Query params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns>Http response message</returns>
        public Task<HttpResponseMessage> SignedDeleteRequestAsync(Uri endpoint,
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
                
        private static Uri CreateValidUri(Uri endpoint, 
            IDictionary<string, string> qParams = default)
        {
            var queryString = ConvertToParamsString(qParams);
            var resultQueryString = $"{endpoint}";
            
            if(queryString != string.Empty)
                resultQueryString = $"{endpoint}?{queryString}";
            
            var uri = new Uri(resultQueryString);
            return uri;
        }

        private Uri CreateValidSignedUri(Uri endpoint,
            IDictionary<string, string> qParams = default)
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