using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class OrderFillPartDtoValidator : AbstractValidator<OrderFillPartDto>
    {
        public OrderFillPartDtoValidator()
        {
            RuleFor(x => x.Qty).Length(0, 51)
                .WithMessage("Допустимая длина строки кол-ва монет со стоимостью от 1 до 50 симловов");
            RuleFor(x => x.Qty).Matches("^\\S+$")
                .WithMessage("Строка с кол-ом монет не может содержать пробелы.");
            RuleFor(x => x.Comission).Length(0, 51)
                .WithMessage("Допустимая длина строки комиссии со стоимостью от 1 до 50 симловов");
            RuleFor(x => x.Comission).Matches("^\\S+$")
                .WithMessage("Строка с комиссией не может содержать пробелы.");
            RuleFor(x => x.ComissionAsset).Length(0, 51)
                .WithMessage("Допустимая длина строки комиссии со стоимостью от 1 до 50 симловов");
            RuleFor(x => x.ComissionAsset).Matches("^\\S+$")
                .WithMessage("Строка с комиссией не может содержать пробелы.");
        }
    }
}