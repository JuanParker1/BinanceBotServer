using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class AuthUserInfoDtoValidator : AbstractValidator<AuthUserInfoDto>
    {
        public AuthUserInfoDtoValidator()
        {
            RuleFor(x => x.Login).NotNull()
                .WithMessage("Логин не должен быть пустым");
            RuleFor(x => x.Login).NotEmpty()
                .WithMessage("Логин не должен быть пустым");
            RuleFor(x => x.Login).Length(0, 31)
                .WithMessage("Допустимая длина логина от 1 до 50 символов");
            RuleFor(x => x.Login).Matches("^\\S+$")
                .WithMessage("Логин не может содержать пробелы.");
            RuleFor(x => x.Name).Length(0, 21)
                .WithMessage("Допустимая длина имени пользователя от 1 до 20 символов");
            RuleFor(x => x.Name).Matches("^\\S+$")
                .WithMessage("Имя не может содержать пробелы.");
            RuleFor(x => x.Surname).Length(0, 21)
                .WithMessage("Допустимая длина фамилии пользователя от 1 до 20 символов");
            RuleFor(x => x.Surname).Matches("^\\S+$")
                .WithMessage("Фамилия не может содержать пробелы.");
            RuleFor(x => x.Email).Length(0, 251)
                .WithMessage("Допустимая длина email от 1 до 250 символов");
            RuleFor(x => x.Email).Matches("^[^\\s+$]([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$")
                .WithMessage("Email должен содержать символ @, а также домен (формата .com/.ru и т.д.). Не может содержать пробелы.");
        }
    }
}