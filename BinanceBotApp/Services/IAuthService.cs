using BinanceBotApp.Data;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface IAuthService
    {
        Task<int> ChangePasswordAsync(int idUser, string newPassword, 
            CancellationToken token);
        Task<AuthUserInfoDto> LoginAsync(string login, string password, 
            CancellationToken token);
        string Refresh(ClaimsPrincipal user);
        Task<bool> RegisterAsync(RegisterDto registerDto, 
            CancellationToken token);
    }
}