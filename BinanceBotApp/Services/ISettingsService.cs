using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface ISettingsService
    {
        Task<int> EnableTradeAsync(int idUser, bool isTradeEnabled,
            CancellationToken token);
        Task<(string apiKey, string secretKey)> GetApiKeysAsync(int idUser,
            CancellationToken token);
        Task<int> SaveApiKeysAsync(ApiKeysDto apiKeysDto, CancellationToken token);
    }
}