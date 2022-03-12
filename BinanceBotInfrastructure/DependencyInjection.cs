using BinanceBotApp.Services;
using BinanceBotApp.Services.BackgroundWorkers;
using BinanceBotDb.Models;
using BinanceBotInfrastructure.Services;
using BinanceBotInfrastructure.Services.BackgroundWorkers;
using BinanceBotInfrastructure.Services.Cache;
using BinanceBotInfrastructure.Services.CoinPricesStorage;
using BinanceBotInfrastructure.Services.WebsocketStorage;
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
            
            services.AddHostedService<PricesBackgroundService>();
            
            services.AddSingleton(new CacheDb()); // TODO: Сделать нормально через интерфейс. Тогда при тестировании ничего мокать не надо будет, просто подставлю нужную затычку через полиморфизм.
            services.AddSingleton<IActiveWebsockets, ActiveWebsockets>();
            services.AddSingleton<ICoinPricesStorage, CoinPricesStorage>();
            services.AddSingleton<IPricesBackgroundQueue, PricesBackgroundQueue>();
            
            services.AddTransient<IRequestTrackerService, RequestTrackerService>();
            services.AddTransient<IAccountBalanceService, AccountBalanceService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ICoinService, CoinService>();
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IWebSocketClientService, WebSocketClientService>();
            services.AddTransient<IAccountBalanceService, AccountBalanceService>();
            services.AddTransient<IEventService, EventService>();

            return services;
        }
    }
}