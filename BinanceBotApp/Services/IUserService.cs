using System;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IUserService
    {
        Task<UserInfoDto> GetUserInfoAsync(int idUser,
            CancellationToken token);
        Task<int> UpdateUserInfoAsync(UserInfoDto userDto, 
            CancellationToken token);
        Task<int> ChangePasswordAsync(ChangePasswordDto changePasswordDto,
            CancellationToken token);
        Task GetUserDataStreamAsync(string listenKey, Action<string> handler,
            CancellationToken token);
    }
}