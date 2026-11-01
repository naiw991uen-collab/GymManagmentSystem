using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.MemberShips.ViewModel;

namespace GymManagmentSystem.BLL.Services.MemberShips.Interfaces;
public interface IMemberShipService
{
    public Task<IEnumerable<MemberShipViewModel>> GetAllMembershipAsync(CancellationToken cancellationToken = default);
    public Task<IEnumerable<PlanSelectListViewModel>> GetAllPlanSelectListAsync(CancellationToken cancellationToken = default);
    public Task<IEnumerable<MemberSelectListViewModel>> GetAllMemberSelectListAsync(CancellationToken cancellationToken = default);
    public Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel model, CancellationToken cancellationToken = default);
    public Task<Result> DeleteActiceMemberShipAsync(int memberId , CancellationToken cancellationToken = default);
}
