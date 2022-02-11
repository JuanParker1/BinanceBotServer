using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class EnableTradeDtoValidator : AbstractValidator<EnableTradeDto>
    {
        public EnableTradeDtoValidator()
        {
            RuleFor(x => x.IdUser).GreaterThan(0)
                .WithMessage("Id пользователя не может быть отрицательным");
        }
    }
}