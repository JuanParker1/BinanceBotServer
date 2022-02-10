using BinanceBotApp.Services;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using BinanceBotInfrastructure.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceBotInfrastructure
{
    public static class DependencyInjection
    {
        public static IBinanceBotDbContext MakeContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
                .UseNpgsql(connectionString)
                .Options;
            var context = new BinanceBotDbContext(options);
            return context;
        }
        
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<BinanceBotDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBinanceBotDbContext, BinanceBotDbContext>();
            services.AddScoped<IHttpClientService, HttpClientService>();
        
            services.AddSingleton(new CacheDb());
            
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ICoinService, CoinService>();
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IWebSocketClientService, WebSocketClientService>();

            return services;
        }
    }
}