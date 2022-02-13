using BinanceBotApp.Data;
using FluentValidation;


namespace BinanceBotApp.DataValidators
{
    public class OrderPriceRateDtoValidator : AbstractValidator<OrderPriceRateDto>
    {
        public OrderPriceRateDtoValidator()
        {
            RuleFor(x => x.IdUser).GreaterThan(0)
                .WithMessage("Id пользователя не может быть отрицательным");
            RuleFor(x => x.OrderPriceRate).Must(rate => rate is >= 10 and <= 30)
                .WithMessage("Уровень установки автоматического лимитного ордера должен быть от 10 до 30" +
                             "процентов включительно");
        }
    }
}