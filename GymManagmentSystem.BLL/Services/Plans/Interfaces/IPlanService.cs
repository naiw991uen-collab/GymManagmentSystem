using GymManagmentSystem.BLL.Services.Plansa.ViewModel;

namespace GymManagmentSystem.BLL.Services.Plansa.Interfaces;
public interface IPlanService
{
    public Task<EditPlanViewModel> GetPlanAsync(int? Id);
    public Task<bool> UpdatePlanAsync(EditPlanViewModel model);
    public Task<bool> DeactivateMemberAsync(int? Id);
}
