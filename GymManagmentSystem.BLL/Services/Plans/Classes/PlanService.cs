using AutoMapper;
using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Plansa.Interfaces;
using GymManagmentSystem.BLL.Services.Plansa.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace GymManagmentSystem.BLL.Services.Plansa.Classes;

public class PlanService : IPlanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PlanService> _logger;

    public PlanService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PlanService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Result<EditPlanViewModel?>> GetPlanAsync(int? Id)
    {
        if (Id == null || Id.Value == 0)
        {
            _logger.LogWarning("GetPlan failed: invalid id");
            return Result<EditPlanViewModel?>.Fail("Id Not Found");
        }

        var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(Id.Value);

        if (plan == null)
        {
            _logger.LogWarning("GetPlan failed: plan {PlanId} not found", Id);
            return Result<EditPlanViewModel?>.Fail("Plan Not Found");
        }

        var model = _mapper.Map<EditPlanViewModel>(plan);

        return Result<EditPlanViewModel?>.OK(model);
    }

    public async Task<Result> UpdatePlanAsync(EditPlanViewModel model)
    {
       try
        {
            if (model == null)
            {
                _logger.LogWarning("UpdatePlan failed: model is null");
                return Result.Fail("Model Not Found");
            }

            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();

            if (plans.Any(x => x.Price == model.Price && x.Id != model.Id))
            {
                _logger.LogWarning("UpdatePlan failed for plan {PlanId}: price {Price} already exists", model.Id, model.Price);
                return Result.Fail("Price Already Exist");
            }

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.Id);

            if (plan == null)
            {
                _logger.LogWarning("UpdatePlan failed: plan {PlanId} not found", model.Id);
                return Result.Fail("Plan Not Found");
            }

            var memberships = await _unitOfWork.GetRepository<MemberShip>().GetAllAsync();

            var hasActiveMemberships = memberships.Any(x => x.PlanId == model.Id && x.EndDate >= DateTime.Now);

            if (hasActiveMemberships)
            {
                _logger.LogWarning("UpdatePlan failed: plan {PlanId} has active memberships", model.Id);
                return Result.Fail("Plan Has Active Memberships");
            }

            _mapper.Map(model,plan);

            await _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Plan {PlanId} updated successfully", model.Id);
                return Result.OK();
            }
            else
            {
                _logger.LogError("UpdatePlan failed for plan {PlanId}: SaveChanges returned 0 rows", model.Id);
                return Result.Fail("Plan Can not be Updated");
            }
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "UpdatePlan threw an unexpected exception for plan {PlanId}", model?.Id);
            return Result.Fail("Internal Server Error");
        }
    }
    public async Task<Result> DeactivateMemberAsync(int? Id)
    {
        try
        {
            if (Id == null || Id.Value == 0)
            {
                _logger.LogWarning("TogglePlanStatus failed: invalid id");
                return Result.Fail("Id Not Found");
            }

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(Id.Value);

            if (plan == null)
            {
                _logger.LogWarning("TogglePlanStatus failed: plan {PlanId} not found", Id);
                return Result.Fail("Plan Not Found");
            }

            plan.IsActive = !plan.IsActive;
            
            await _unitOfWork.GetRepository<Plan>().UpdateAsync(plan);

            var result = await _unitOfWork.SaveChangesAsync() > 0;

            if (result)
            {
                _logger.LogInformation("Plan {PlanId} status changed, IsActive is now {IsActive}", Id, plan.IsActive);
                return Result.OK();
            }
            else
            {
                _logger.LogError("TogglePlanStatus failed for plan {PlanId}: SaveChanges returned 0 rows", Id);
                return Result.Fail("Plan Status Can not be Changed");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "TogglePlanStatus threw an unexpected exception for plan {PlanId}", Id);
            return Result.Fail("Internal Server Error");
        }
    }
}
