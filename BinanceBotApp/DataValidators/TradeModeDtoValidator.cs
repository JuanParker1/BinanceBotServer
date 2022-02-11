using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class TradeModeDtoValidator : AbstractValidator<TradeModeDto>
    {
        public TradeModeDtoValidator()
        {
            RuleFor(x => x.IdUser).GreaterThan(0)
                .WithMessage("Id пользователя не может быть отрицательным");
            RuleFor(x => x.IdTradeMode).Must(id => id is 1 or 2)
                .WithMessage("Id режима торговли может иметь значение только 1 или 2");
        }
    }
}