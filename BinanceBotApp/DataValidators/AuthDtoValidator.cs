using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class AuthDtoValidator : AbstractValidator<AuthDto> 
    {
        public AuthDtoValidator()
        {
            RuleFor(x => x.Login).NotNull()
                .WithMessage("Логин не должен быть пустым");
            RuleFor(x => x.Login).NotEmpty()
                .WithMessage("Логин не должен быть пустым");
            RuleFor(x => x.Login).Length(0, 31)
                .WithMessage("Допустимая длина логина от 1 до 30 символов");
            RuleFor(x => x.Password).NotNull()
                .WithMessage("Пароль не должен быть пустым");
            RuleFor(x => x.Password).NotEmpty()
                .WithMessage("Пароль не должен быть пустым");
            RuleFor(x => x.Password).Length(0, 21)
                .WithMessage("Допустимая длина пароля от 1 до 30 символов");
        }
    }
}