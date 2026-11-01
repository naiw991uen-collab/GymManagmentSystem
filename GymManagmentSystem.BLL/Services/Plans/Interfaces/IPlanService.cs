using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Plansa.ViewModel;

namespace GymManagmentSystem.BLL.Services.Plansa.Interfaces;
public interface IPlanService
{
    public Task<Result<EditPlanViewModel?>> GetPlanAsync(int? Id);
    public Task<Result> UpdatePlanAsync(EditPlanViewModel model);
    public Task<Result> DeactivateMemberAsync(int? Id);
}
