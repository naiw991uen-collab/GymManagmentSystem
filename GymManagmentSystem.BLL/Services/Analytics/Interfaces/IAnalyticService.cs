using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Analytics.ViewModel;

namespace GymManagmentSystem.BLL.Services.Analytics.Interfaces;
public interface IAnalyticService
{
    public Task<Result<AnalyticViewModel>> GetDataAsync();
}
