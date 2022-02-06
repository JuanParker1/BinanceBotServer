using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface ISettingsService
    {
        Task<int> EnableTradeAsync(int idUser, bool isTradeEnabled,
            CancellationToken token);
    }
}