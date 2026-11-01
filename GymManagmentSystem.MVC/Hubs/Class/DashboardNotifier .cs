using GymManagmentSystem.BLL.Services.Analytics.Interfaces;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.SignalR;

namespace GymManagmentSystem.PL.Hubs.Class;
public class DashboardNotifier : IDashboardNotifier
{
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly IAnalyticService _analyticService;

    public DashboardNotifier(IHubContext<DashboardHub> hubContext, IAnalyticService analyticService)
    {
        _hubContext = hubContext;
        _analyticService = analyticService;
    }

    public async Task NotifyStatsChangedAsync()
    {
        var result = await _analyticService.GetDataAsync();

        if (!result.Success)   
            return;

        await _hubContext.Clients.All.SendAsync("UpdateStats", result.Value);   
    }
}
