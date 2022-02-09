using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class CreatedOrderResultDtoValidator : AbstractValidator<CreatedOrderResultDto>
    {
        public CreatedOrderResultDtoValidator()
        {
            RuleFor(x => x.Status).Length(0, 51)
                .WithMessage("Допустимая длина статуса от 1 до 50 символов");
            RuleFor(x => x.TimeInForce).Length(0, 21)
                .WithMessage("Допустимая длина строки времени жизни ордера от 1 до 20 симловов");
            RuleFor(x => x.Type).Length(0, 21)
                .WithMessage("Допустимая длина типа ордера от 1 до 20 символов");
            RuleFor(x => x.Side).Length(0, 11)
                .WithMessage("Допустимая длина названия торговой пары от 1 до 10 символов");
        }
    }
}