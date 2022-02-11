using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Endpoints;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services.Cache;
using Mapster;

namespace BinanceBotInfrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly CacheTable<Settings> _cacheUserSettings;
        private readonly IHttpClientService _httpService;
        
        public SettingsService(IBinanceBotDbContext db, CacheDb cacheDb,
            IHttpClientService httpService)
        {
            _cacheUserSettings = cacheDb.GetCachedTable<Settings>((BinanceBotDbContext)db, 
                new HashSet<string> {"TradeMode"});
            _httpService = httpService;
        }

        public async Task<SettingsDto> GetSettingsAsync(int idUser, CancellationToken token)
        {
            var userSettings = await _cacheUserSettings.FirstOrDefaultAsync(c => 
                c.IdUser == idUser, token);
       
            if (userSettings is null)
                return null;
            
            var userSettingsDto = userSettings.Adapt<SettingsDto>();

            userSettingsDto.IsApiKeysSet = !string.IsNullOrEmpty(userSettings.ApiKey) &&
                                           !string.IsNullOrEmpty(userSettings.SecretKey);

            userSettingsDto.TradeMode = userSettings.TradeMode.Caption;

            return userSettingsDto;
        }

        public async Task<int> EnableTradeAsync(int idUser, bool isTradeEnabled, 
            CancellationToken token)
        {
            var userSettings = await _cacheUserSettings.FirstOrDefaultAsync(s => 
                    s.IdUser == idUser, token);

            if (userSettings is null) 
                return 0;
            
            // TODO: Стопнуть здесь из другого сервиса всю BackgroundWorker торговлю

            userSettings.IsTradeEnabled = isTradeEnabled;

            return await _cacheUserSettings.UpsertAsync(userSettings, token);
        }

        public async Task<int> SaveTradeModeAsync(int idUser, int idTradeMode, 
            CancellationToken token)
        {
            var userSettings = await _cacheUserSettings.FirstOrDefaultAsync(s => 
                s.IdUser == idUser, token);

            if (userSettings is null) 
                return 0;

            userSettings.IdTradeMode = idTradeMode;
            
            return await _cacheUserSettings.UpsertAsync(userSettings, token);
        }

        public async Task<int> ChangeOrderPriceRateAsync(int idUser, int orderPriceRate,
            CancellationToken token)
        {
            var userSettings = await _cacheUserSettings.FirstOrDefaultAsync(s => 
                s.IdUser == idUser, token);

            if (userSettings is null) 
                return 0;

            userSettings.LimitOrderRate = orderPriceRate;
            
            return await _cacheUserSettings.UpsertAsync(userSettings, token);
        }

        public async Task<(string apiKey, string secretKey)> GetApiKeysAsync(int idUser,
            CancellationToken token)
        {
            var userSettings = await _cacheUserSettings.FirstOrDefaultAsync(s => 
                s.IdUser == idUser, token);

            return userSettings is null 
                ? default 
                : (userSettings.ApiKey, userSettings.SecretKey);
        }
        
        public async Task<int> SaveApiKeysAsync(ApiKeysDto apiKeysDto,
            CancellationToken token)
        {
            var userSettings = await _cacheUserSettings.FirstOrDefaultAsync(s =>
                s.IdUser == apiKeysDto.IdUser, token);

            if (userSettings is null)
                return 0;

            userSettings.ApiKey = apiKeysDto.ApiKey.Trim();
            userSettings.SecretKey = apiKeysDto.SecretKey.Trim();

            return await _cacheUserSettings.UpsertAsync(userSettings, token);
        }
        
        public async Task<string> GetListenKey(CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
        
            var listenKey = await _httpService.ProcessRequestAsync<string>(uri,
                null, default, HttpMethods.SignedPost, token);
        
            return listenKey;
        }
        
        public async Task ExtendListenKey(string listenKey, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
        
            await _httpService.ProcessRequestAsync<string>(uri,
                new Dictionary<string, string>(), 
                default, HttpMethods.SignedPut, token);
        }
        
        public async Task DeleteListenKey(string listenKey, CancellationToken token)
        {
            var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
        
            await _httpService.ProcessRequestAsync<string>(uri,
                new Dictionary<string, string>(), 
                default, HttpMethods.SignedDelete, token);
        }
    }
}