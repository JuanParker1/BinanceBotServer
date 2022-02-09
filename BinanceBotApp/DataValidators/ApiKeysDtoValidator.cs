using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators
{
    public class ApiKeysDtoValidator : AbstractValidator<ApiKeysDto>
    {
        public ApiKeysDtoValidator()
        {
            RuleFor(x => x.IdUser).GreaterThan(0)
                .WithMessage("Id пользователя не может быть отрицательным");
            RuleFor(x => x.ApiKey).NotNull()
                .WithMessage("Api ключ не должен быть пустым");
            RuleFor(x => x.ApiKey).NotEmpty()
                .WithMessage("Api ключ не должен быть пустым");
            RuleFor(x => x.ApiKey).Length(0, 51)
                .WithMessage("Допустимая длина api ключа от 1 до 50 символов");
            RuleFor(x => x.SecretKey).NotNull()
                .WithMessage("Секретный ключ не должен быть пустым");
            RuleFor(x => x.SecretKey).NotEmpty()
                .WithMessage("Секретный ключ не должен быть пустым");
            RuleFor(x => x.SecretKey).Length(0, 51)
                .WithMessage("Допустимая длина секретного ключа от 1 до 50 символов");
        }
    }
}