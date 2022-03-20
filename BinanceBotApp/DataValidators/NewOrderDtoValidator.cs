using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class NewOrderDtoValidator : AbstractValidator<NewOrderDto>
    {
        public NewOrderDtoValidator()
        {
            RuleFor(x => x.IdUser).GreaterThan(0)
                .WithMessage("Id пользователя не может быть отрицательным");
            RuleFor(x => x.Symbol).NotNull()
                .WithMessage("Название торговой пары не должен быть пустым");
            RuleFor(x => x.Symbol).NotEmpty()
                .WithMessage("Название торговой пары не должен быть пустым");
            RuleFor(x => x.Symbol).Length(0, 21)
                .WithMessage("Допустимая длина направления ордера от 1 до 20 символов");
            RuleFor(x => x.Symbol).Matches("^\\S+$")
                .WithMessage("Название торовой пары не может содержать пробелы.");
            RuleFor(x => x.Side).NotNull()
                .WithMessage("Название направления ордера не может быть пустым");
            RuleFor(x => x.Side).NotEmpty()
                .WithMessage("Название направления ордера не может быть пустым");
            RuleFor(x => x.Side).Length(0, 11)
                .WithMessage("Допустимая длина названия торговой пары от 1 до 10 символов");
            RuleFor(x => x.Side).Matches("^\\S+$")
                .WithMessage("Направление ордера не может содержать пробелы.");
            RuleFor(x => x.Type).NotNull()
                .WithMessage("Тип ордера не может быть пустым");
            RuleFor(x => x.Type).NotEmpty()
                .WithMessage("Тип ордера не может быть пустым");
            RuleFor(x => x.Type).Length(0, 21)
                .WithMessage("Допустимая длина типа ордера от 1 до 20 символов");
            RuleFor(x => x.Type).Matches("^\\S+$")
                .WithMessage("Ти ордера не может содержать пробелы.");
            RuleFor(x => x.NewClientOrderId).Length(0, 31)
                .WithMessage("Допустимая длина cтроки id нового ордера от 1 до 30 симловов");
            RuleFor(x => x.NewClientOrderId).Matches("^\\S+$")
                .WithMessage("Строка id нового ордера не может содержать пробелы.");
            RuleFor(x => x.NewOrderRespType).Length(0, 31)
                .WithMessage("Допустимая длина cтроки с типом нового ордера от 1 до 30 симловов");
            RuleFor(x => x.NewOrderRespType).Matches("^\\S+$")
                .WithMessage("Строка с типом нового ордера не может содержать пробелы.");
        }
    }
}