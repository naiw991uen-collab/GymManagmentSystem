using GymManagmentSystem.BLL.Services.Plansa.Interfaces;
using GymManagmentSystem.BLL.Services.Plansa.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;

namespace GymManagmentSystem.BLL.Services.Plansa.Classes;

public class PlanService : IPlanService
{
    private readonly IUnitOfWork _unitOfWork;

    public PlanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<EditPlanViewModel> GetPlanAsync(int? Id)
    {
            if (Id == null || Id.Value == 0)
                return null;

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(Id.Value);

            if (plan == null)
                return null;

            var model = new EditPlanViewModel()
            {
                Id = plan.Id,
                PlanName = plan.Name,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                Description = plan.Description,
            };

            return model;
    }

    public async Task<bool> UpdatePlanAsync(EditPlanViewModel model)
    {
       try
        {
            if (model == null)
                return false;

            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();

            //if (plans.Any(x => x.DurationDays == model.DurationDays && x.Id != model.Id))
            //    return false;

            if (plans.Any(x => x.Price == model.Price && x.Id != model.Id))
                return false;

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.Id);

            if (plan == null)
                return false;

            var memberships = await _unitOfWork.GetRepository<MemberShip>().GetAllAsync();

            var hasActiveMemberships = memberships.Any(x => x.PlanId == model.Id && x.EndDate >= DateTime.Now);

            if (hasActiveMemberships)
                return false;

            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.Description = model.Description;

            await _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool> DeactivateMemberAsync(int? Id)
    {
        try
        {
            if (Id == null || Id.Value == 0)
                return false;

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(Id.Value);

            if (plan == null)
                return false;

            plan.IsActive = !plan.IsActive;
            await _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        catch
        {
            return false;
        }
    }
}
