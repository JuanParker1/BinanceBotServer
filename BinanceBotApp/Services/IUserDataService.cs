using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface IUserDataService
    {
        Task<string> GetListenKey(CancellationToken token);
        Task ExtendListenKey(string listenKey, CancellationToken token);
        Task DeleteListenKey(string listenKey, CancellationToken token);
    }
}