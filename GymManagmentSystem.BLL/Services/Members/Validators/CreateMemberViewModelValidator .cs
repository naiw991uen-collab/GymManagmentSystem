using FluentValidation;
using GymManagmentSystem.BLL.Services.Members.ViewModel;

namespace GymManagmentSystem.BLL.Services.Members.Validators;
public class CreateMemberViewModelValidator : AbstractValidator<CreateMemberViewModel>
{
     public CreateMemberViewModelValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name Is Required")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain letters and spaces");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email Is Required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone Number Is Required")
            .Matches(@"^(010|011|012|015)\d{8}$")
            .WithMessage("Phone number must be a valid Egyptian mobile number");

        RuleFor(x => x.DateOfBirth)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Date of Birth is required")
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of Birth must be in the past");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender is required");

        RuleFor(x => x.BuildingNumber)
            .InclusiveBetween(1, 9000)
            .WithMessage("Building Number must be between 1 and 9000");

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

        RuleFor(x => x.HealthRecordViewModel)
            .NotNull().WithMessage("Health record is required")
            .SetValidator(new HealthRecordViewModelValidator());
    }
}
