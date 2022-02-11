using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ISettingsService
    {
        Task<SettingsDto> GetSettingsAsync(int idUser, CancellationToken token);
        Task<int> EnableTradeAsync(EnableTradeDto enableTradeDto,
            CancellationToken token);
        Task<int> SaveTradeModeAsync(TradeModeDto tradeModeDto,
            CancellationToken token);
        Task<int> ChangeOrderPriceRateAsync(int idUser, int orderPriceRate,
            CancellationToken token);
        Task<(string apiKey, string secretKey)> GetApiKeysAsync(int idUser,
            CancellationToken token);
        Task<int> SaveApiKeysAsync(ApiKeysDto apiKeysDto, CancellationToken token);
    }
}