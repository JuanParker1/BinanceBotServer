using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceBotInfrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<BinanceBotDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBinanceBotDbContext>(provider => 
                provider.GetService<BinanceBotDbContext>());
            
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ICoinService, CoinService>();
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<IHttpClientService>(s => new HttpClientService("RICK4hQQ82ClQxuxFEhcYP0KY3057uioNKwdnwpxUVce96fRYFwh4ApK80U0vhQq",
                "YeVKxVSiGtdu6cDawkn8f6kwgvRaYRN5qkXeU169lNwSM749pb8KCTzJGArLGtqd"));
            services.AddTransient<IWebSocketClientService, WebSocketClientService>();
            services.AddTransient<IUserDataService, UserDataService>();

            return services;
        }
    }
}