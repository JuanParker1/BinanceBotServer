using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators.Dto
{
    public class CreatedOrderAckDtoValidator : AbstractValidator<CreatedOrderAckDto>
    {
        public CreatedOrderAckDtoValidator()
        {
            RuleFor(x => x.Symbol).NotNull()
                .WithMessage("Название торговой пары не должен быть пустым");
            RuleFor(x => x.Symbol).NotEmpty()
                .WithMessage("Название торговой пары не должен быть пустым");
            RuleFor(x => x.Symbol).Length(0, 21)
                .WithMessage("Допустимая длина направления ордера от 1 до 20 символов");
            RuleFor(x => x.ClientOrderId).Length(0, 31)
                .WithMessage("Допустимая длина cтроки id ордера от 1 до 30 симловов");
        }
    }
}