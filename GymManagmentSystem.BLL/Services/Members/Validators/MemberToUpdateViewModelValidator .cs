using FluentValidation;
using GymManagmentSystem.BLL.Services.Members.ViewModel;

namespace GymManagmentSystem.BLL.Services.Members.Validators;
public class MemberToUpdateViewModelValidator : AbstractValidator<MemberToUpdateViewModel>
{
    public MemberToUpdateViewModelValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email Is Required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone Number Is Required")
            .Matches(@"^(010|011|012|015)\d{8}$")
            .WithMessage("Phone number must be a valid Egyptian mobile number");

        RuleFor(x => x.BuildingNumber)
            .GreaterThan(0)
            .WithMessage("Building Number must be greater than 0");

        RuleFor(x => x.City)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("City Is Required")
            .Length(2, 100).WithMessage("City must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("City can only contain letters and spaces");

        RuleFor(x => x.Street)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Street Is Required")
            .Length(2, 150).WithMessage("Street must be between 2 and 150 characters")
            .Matches(@"^[a-zA-Z0-9\s]+$")
            .WithMessage("Street can only contain letters, numbers, and spaces");
    }
}
