using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IBinanceBotDbContext _db;
        private readonly IHttpClientService _httpClientService;
        
        public SettingsService(IBinanceBotDbContext db, IHttpClientService httpClientService)
        {
            _db = db;
            _httpClientService = httpClientService;
        }

        public async Task<int> EnableTradeAsync(int idUser, bool isTradeEnabled, 
            CancellationToken token)
        {
            var userSettings = await _db.UserSettings.FirstOrDefaultAsync(s => s.IdUser == idUser,
                token);

            if (userSettings is null) 
                return 0;
            
            // TODO: Стопнуть здесь из другого сервиса всю BackgroundWorker торговлю
            
            // TODO: А еще надо реализовывать кэш, потому что многие настройки будут управляться именно через него.
            
            userSettings.IsTradeEnabled = isTradeEnabled;

            return await _db.SaveChangesAsync(token);
        }
        
        public async Task<string> GetListenKey(CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
        
            var listenKey = await _httpClientService.ProcessRequestAsync<string>(uri,
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
    }
}