using BinanceBotApp.Data;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace BinanceBotApp.DataValidators
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidation();
            
            services.AddTransient<IValidator<ApiKeysDto>, ApiKeysDtoValidator>();
            services.AddTransient<IValidator<AuthDto>, AuthDtoValidator>();
            services.AddTransient<IValidator<UserInfoDto>, AuthUserInfoDtoValidator>();
            services.AddTransient<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();
            services.AddTransient<IValidator<SwitchTradeDto>, SwitchTradeDtoValidator>();
            services.AddTransient<IValidator<NewOrderDto>, NewOrderDtoValidator>();
            services.AddTransient<IValidator<OrderPriceRateDto>, OrderPriceRateDtoValidator>();
            services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
            services.AddTransient<IValidator<TradeModeDto>, TradeModeDtoValidator>();

            return services;
        }
    }
}