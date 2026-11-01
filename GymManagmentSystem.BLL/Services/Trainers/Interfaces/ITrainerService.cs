using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;
namespace GymManagmentSystem.BLL.Services.Trainers.Interfaces;
public interface ITrainerService
{
    public Task<Result<IEnumerable<TrainerViewMoldel>>> GetTrainersAsync();
    public Task<Result> AddTrainerAsyn(CreateTrainerViewModel model);
    public Task<Result<TrainerDetailsViewModel?>> GetTrainerAsync(int? Id);
    public Task<Result<TrainerToUpdateViewModel?>> GetTrainerToUpdateAsync(int? Id);
    public Task<Result> UpdateTrainerAsync(TrainerToUpdateViewModel model);
    public Task<Result> DeleteTrainerAsync(int? Id);
}
