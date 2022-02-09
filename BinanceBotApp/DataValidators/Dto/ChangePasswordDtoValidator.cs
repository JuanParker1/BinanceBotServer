using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators.Dto
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.OldPassword).NotNull()
                .WithMessage("Старый пароль не должен быть пустым");
            RuleFor(x => x.OldPassword).NotEmpty()
                .WithMessage("Старый пароль не должен быть пустым");
            RuleFor(x => x.OldPassword).Length(0, 21)
                .WithMessage("Допустимая длина старого пароля от 1 до 30 символов");
            RuleFor(x => x.NewPassword).NotNull()
                .WithMessage("Новый пароль не должен быть пустым");
            RuleFor(x => x.NewPassword).NotEmpty()
                .WithMessage("Новый пароль не должен быть пустым");
            RuleFor(x => x.NewPassword).Length(0, 21)
                .WithMessage("Допустимая длина нового пароля от 1 до 30 символов");
        }
    }
}