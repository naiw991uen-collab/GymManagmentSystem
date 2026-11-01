using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
namespace GymManagmentSystem.BLL.Services.Members.Interfaces;
public interface IMemberService
{
    public Task<Result<IEnumerable<MemberViewModel>>> GetMembersAsyn();
    public Task<Result> AddMemberAsync(CreateMemberViewModel model);
    public Task<Result<MemberDetailsViewModel?>> GetMemberAsync(int? Id);
    public Task<Result<HealthRecordViewModel?>> GetHealthRecordAsync(int? Id);
    public Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int? Id);
    public Task<Result> UpdateMemberAsync(MemberToUpdateViewModel model);
    public Task<Result> DeleteMemberAsync(int? Id);
}
