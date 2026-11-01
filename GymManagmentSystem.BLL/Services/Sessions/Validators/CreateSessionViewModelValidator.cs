using FluentValidation;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;

namespace GymManagmentSystem.BLL.Services.Sessions.Validators;
public class CreateSessionViewModelValidator : AbstractValidator<CreateSessionViewModel>
{
    public CreateSessionViewModelValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .Length(10, 500).WithMessage("Description must be between 10 and 500 characters");

        RuleFor(x => x.Capacity)
            .InclusiveBetween(0, 25).WithMessage("Capacity must be between 0 and 25");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .WithName("Start Date & Time");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date")
            .WithName("End Date & Time");

        RuleFor(x => x.TrainerId)
            .GreaterThan(0).WithMessage("Trainer is required")
            .WithName("Trainer");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category is required")
            .WithName("Category");
    }
}
