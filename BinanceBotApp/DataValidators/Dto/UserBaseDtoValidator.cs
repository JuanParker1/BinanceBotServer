using BinanceBotApp.Data;
using FluentValidation;

namespace BinanceBotApp.DataValidators.Dto
{
    public class UserBaseDtoValidator : AbstractValidator<UserBaseDto>
    {
        public UserBaseDtoValidator()
        {
            RuleFor(x => x.Login).NotNull()
                .WithMessage("Логин не должен быть пустым");
            RuleFor(x => x.Login).NotEmpty()
                .WithMessage("Логин не должен быть пустым");
            RuleFor(x => x.Login).Length(0, 31)
                .WithMessage("Допустимая длина логина от 1 до 50 символов");
            RuleFor(x => x.Name).Length(0, 21)
                .WithMessage("Допустимая длина имени пользователя от 1 до 20 символов");
            RuleFor(x => x.Surname).Length(0, 21)
                .WithMessage("Допустимая длина фамилии пользователя от 1 до 20 символов");
            RuleFor(x => x.Email).Length(0, 251)
                .WithMessage("Допустимая длина email от 1 до 250 символов");
        }
    }
}