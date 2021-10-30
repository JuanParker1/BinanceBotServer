using System;
using System.Text.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    public class HttpResponseService : IHttpResponseService
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public HttpResponseService()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
        
        public async Task<T> HandleResponseAsync<T>(HttpResponseMessage message, 
            CancellationToken token = default) where T : class
        {
            if (message is null) 
                throw new ArgumentNullException("Message is null");
            
            if (!message.IsSuccessStatusCode) 
                throw new Exception($"Request not successfull. " +
                                    $"Error status code: {message.StatusCode}");
            
            var messageJson = await message.Content.ReadAsStringAsync(token)
                .ConfigureAwait(false);

            var messageObject = JsonSerializer.Deserialize<T>(messageJson, _jsonSerializerOptions);
            if (messageObject is not null) 
                return messageObject;
            
            var deserializeErrorMessage = "Unable to deserialize http message to: " +
                                          $"{nameof(T)}.";
            throw new Exception(deserializeErrorMessage);
        }
    }
}