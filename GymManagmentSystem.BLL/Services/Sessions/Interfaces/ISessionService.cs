using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;

namespace GymManagmentSystem.BLL.Services.Sessions.Interfaces;
public interface ISessionService
{
    public Task<Result<IEnumerable<SessionViewModel>?>> GetAllSessionsAsync();
    public Task<Result> CreateSessionAsync(CreateSessionViewModel model);

    public Task<Result<IEnumerable<TrainerSelectedViewModel>>> GetTrainersForDropDownAsync();
    public Task<Result<IEnumerable<CategorySelectedViewModel>>> GetCategoriesForDropDownAsync();
    public Task<Result<SessionDetailsViewModel?>> GetSessionDetailsAsync(int? Id);
    public Task<Result<SessionToUpdateViewModel?>> GetSessionToUpdateAsync(int? Id);
    public Task<Result> UpdateSessionAsync(SessionToUpdateViewModel model);
    public Task<Result> DeleteSessionAsync(int? Id);
}
