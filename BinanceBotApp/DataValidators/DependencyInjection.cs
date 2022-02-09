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
            services.AddTransient<IValidator<CreatedOrderAckDto>, CreatedOrderAckDtoValidator>();
            services.AddTransient<IValidator<CreatedOrderFullDto>, CreatedOrderFullDtoValidator>();
            services.AddTransient<IValidator<CreatedOrderResultDto>, CreatedOrderResultDtoValidator>();
            services.AddTransient<IValidator<DeletedOrderDto>, DeletedOrderDtoValidator>();
            services.AddTransient<IValidator<NewOrderDto>, NewOrderDtoValidator>();
            services.AddTransient<IValidator<OrderFillPartDto>, OrderFillPartDtoValidator>();
            services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
            services.AddTransient<IValidator<UserBaseDto>, UserBaseDtoValidator>();

            return services;
        }
    }
}