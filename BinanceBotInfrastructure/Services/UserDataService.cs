using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;

namespace BinanceBotInfrastructure.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IWebSocketClientService _wsClientService;
        public UserDataService(IHttpClientService httpClientService, IWebSocketClientService wsClientService)
        {
            _httpClientService = httpClientService;
            _wsClientService = wsClientService;
        }
        
        public async Task<ListenKeyDto> GetListenKey(CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();

            var listenKey = await _httpClientService.ProcessRequestAsync<ListenKeyDto>(uri,
                null, HttpMethods.SignedPost, token);

            return listenKey;
        }
        
        public async Task ExtendListenKey(string listenKey, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();

            await _httpClientService.ProcessRequestAsync<string>(uri,
                new Dictionary<string, string>(), HttpMethods.SignedPut, token);
        }
        
        public async Task DeleteListenKey(string listenKey, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();

            await _httpClientService.ProcessRequestAsync<string>(uri,
                new Dictionary<string, string>(), HttpMethods.SignedDelete, token);
        }

        public async Task SubscribeForStreamAsync(string listenKey, Action<string> handler,
            CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetUserDataStreamEndpoint(listenKey);
            
            await _wsClientService.ConnectToWebSocketAsync(uri, "", Console.WriteLine, token );
        }
    }
}