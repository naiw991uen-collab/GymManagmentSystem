using FluentValidation;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;

namespace GymManagmentSystem.BLL.Services.Trainers.Validators;
public class TrainerToUpdateViewModelValidator : AbstractValidator<TrainerToUpdateViewModel>
{
    public TrainerToUpdateViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email can't exceed 100 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^(010|011|012|015)\d{8}$").WithMessage("Phone must be a valid Egyptian number");

        RuleFor(x => x.BuildingNumber)
            .InclusiveBetween(1, 9000).WithMessage("Building Number must be between 1 and 9000");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .Length(2, 150).WithMessage("Street must be between 2 and 150 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .Length(2, 100).WithMessage("City must be between 2 and 100 characters");

        RuleFor(x => x.Specialty)
            .IsInEnum().WithMessage("Please select a specialty");
    }
}
