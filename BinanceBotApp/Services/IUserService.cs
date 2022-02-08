using System;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IUserService
    {
        Task<int> UpdateUserInfoAsync(UserBaseDto userDto, CancellationToken token);
        Task GetUserDataStreamAsync(string listenKey, Action<string> handler,
            CancellationToken token);
    }
}