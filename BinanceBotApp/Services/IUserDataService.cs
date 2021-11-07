using System;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IUserDataService
    {
        Task<ListenKeyDto> GetListenKey(CancellationToken token);
        Task ExtendListenKey(string listenKey, CancellationToken token);
        Task DeleteListenKey(string listenKey, CancellationToken token);
        Task SubscribeForStreamAsync(string listenKey, Action<string> handler,
            CancellationToken token);
    }
}