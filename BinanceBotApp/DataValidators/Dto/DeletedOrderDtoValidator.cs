using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators.Dto
{
    public class DeletedOrderDtoValidator : AbstractValidator<DeletedOrderDto>
    {
        public DeletedOrderDtoValidator()
        {
            RuleFor(x => x.OrigClientOrderId).Length(0, 51)
                .WithMessage("Допустимая длина id отмененного ордера от 1 до 50 символов");
        }
    }
}