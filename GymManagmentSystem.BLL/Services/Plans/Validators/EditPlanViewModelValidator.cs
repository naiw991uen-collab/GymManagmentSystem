using FluentValidation;
using GymManagmentSystem.BLL.Services.Plansa.ViewModel;

namespace GymManagmentSystem.BLL.Services.Plans.Validators;
public class EditPlanViewModelValidator : AbstractValidator<EditPlanViewModel>
{
    public EditPlanViewModelValidator()
    {
        RuleFor(x => x.DurationDays)
            .InclusiveBetween(1, 365)
            .WithName("Duration (Days)");

        RuleFor(x => x.Price)
            .InclusiveBetween(0.01m, 10000m)
            .WithName("Price (EGP)");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);
    }
}
