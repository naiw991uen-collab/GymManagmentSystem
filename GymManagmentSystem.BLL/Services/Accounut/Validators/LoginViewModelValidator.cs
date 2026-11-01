using FluentValidation;
using GymManagmentSystem.BLL.Services.Accounut.ViewModel;

namespace GymManagmentSystem.BLL.Services.Accounut.Validators;
public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    public LoginViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email Is Required")
            .EmailAddress().WithMessage("Invalid Email Format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password Is Required");
    }
}
