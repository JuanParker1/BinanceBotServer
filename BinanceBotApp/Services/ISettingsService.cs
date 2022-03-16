using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ISettingsService
    {
        Task<SettingsDto> GetSettingsAsync(int idUser, CancellationToken token);
        Task<int> SwitchTradeAsync(SwitchTradeDto switchTradeDto,
            CancellationToken token);
        Task<int> SaveTradeModeAsync(TradeModeDto tradeModeDto,
            CancellationToken token);
        Task<int> ChangeOrderPriceRateAsync(OrderPriceRateDto orderPriceRateDto,
            CancellationToken token);
        Task<(string apiKey, string secretKey)> GetApiKeysAsync(int idUser,
            CancellationToken token);
        Task<int> SaveApiKeysAsync(ApiKeysDto apiKeysDto, CancellationToken token);
    }
}