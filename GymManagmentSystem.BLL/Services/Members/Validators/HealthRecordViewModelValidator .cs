using FluentValidation;
using GymManagmentSystem.BLL.Services.Members.ViewModel;

namespace GymManagmentSystem.BLL.Services.Members.Validators;
public class HealthRecordViewModelValidator : AbstractValidator<HealthRecordViewModel>
{
    public HealthRecordViewModelValidator()
    {
        RuleFor(x => x.Height)
            .InclusiveBetween(0.1m, 300m)
            .WithMessage("Height must be between 0.1 and 300");

        RuleFor(x => x.Weight)
            .InclusiveBetween(0.1m, 500m)
            .WithMessage("Weight must be between 0.1 and 500");

        RuleFor(x => x.BloodType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Blood Type Is Required")
            .MaximumLength(3).WithMessage("Blood type must be 3 characters or less");
    }
}
