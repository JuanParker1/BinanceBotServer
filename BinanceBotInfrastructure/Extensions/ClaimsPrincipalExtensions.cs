using System.Security.Claims;
using BinanceBotApp.Data;

namespace BinanceBotInfrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(nameof(RegisterDto.Id));
            if (userId is null)
                return null;

            return int.TryParse(userId.Value, out var uid)
                ? uid
                : null;
        }
    }
}