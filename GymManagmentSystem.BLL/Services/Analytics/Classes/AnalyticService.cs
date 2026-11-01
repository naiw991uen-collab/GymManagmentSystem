using GymManagmentSystem.BLL.Common;
using GymManagmentSystem.BLL.Services.Analytics.Interfaces;
using GymManagmentSystem.BLL.Services.Analytics.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagmentSystem.BLL.Services.Analytics.Classes;

public class AnalyticService : IAnalyticService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AnalyticService> _logger;
    public AnalyticService(IUnitOfWork unitOfWork, ILogger<AnalyticService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AnalyticViewModel>> GetDataAsync()
    {
        try
        {
            var data = new AnalyticViewModel()
            {
                TotalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(),
                ActiveMembers = await _unitOfWork.GetRepository<MemberShip>().CountAsync(x => x.EndDate > DateTime.Now),
                TotalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(),
                UpcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(x => x.StartDate > DateTime.Now),
                OngoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now),
                CompletedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(x => x.EndDate < DateTime.Now),
            };

            _logger.LogInformation(
                    "Analytics fetched: Members={Members}, Trainers={Trainers}, Upcoming={Upcoming}",
                    data.TotalMembers, data.TotalTrainers, data.UpcomingSessions);

            return Result<AnalyticViewModel>.OK(data);
        }
     catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch analytics data");
            return Result<AnalyticViewModel>.Fail("Failed to load analytics");  
        }
    }
}
