using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class SwitchTradeDtoValidator : AbstractValidator<SwitchTradeDto>
    {
        public SwitchTradeDtoValidator()
        {
            RuleFor(x => x.IdUser).GreaterThan(0)
                .WithMessage("Id пользователя не может быть отрицательным");
        }
    }
}