using GymManagmentSystem.BLL.Services.Members.ViewModel;
namespace GymManagmentSystem.BLL.Services.Members.Interfaces;
public interface IMemberService
{
    public Task<IEnumerable<MemberViewModel>> GetMembersAsyn();
    public Task<bool> AddMemberAsync(CreateMemberViewModel model);
    public Task<MemberDetailsViewModel?> GetMemberAsync(int? Id);
    public Task<HealthRecordViewModel?> GetHealthRecordAsync(int? Id);
    public Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int? Id);
    public Task<bool> UpdateMemberAsync(MemberToUpdateViewModel model);
    public Task<bool> DeleteMemberAsync(int? Id);
}
