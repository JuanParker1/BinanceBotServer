using System;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IUserService
    {
        Task<int> UpdateUserInfoAsync(UserBaseDto userDto,
            CancellationToken token);
        Task<int> SaveApiKeysAsync(ApiKeysDto apiKeysDto,
            CancellationToken token);
        Task<string> GetListenKey(CancellationToken token);
        Task ExtendListenKey(string listenKey, CancellationToken token);
        Task DeleteListenKey(string listenKey, CancellationToken token);
        Task GetUserDataStreamAsync(string listenKey, Action<string> handler,
            CancellationToken token);
    }
}