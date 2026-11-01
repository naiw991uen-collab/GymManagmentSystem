using GymManagmentSystem.BLL.Services.Analytics.Interfaces;
using GymManagmentSystem.BLL.Services.Analytics.ViewModel;
using GymManagmentSystem.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymManagmentSystem.MVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAnalyticService _analyticService;
    public HomeController(ILogger<HomeController> logger, IAnalyticService analyticService)
    {
        _logger = logger;
        _analyticService = analyticService;
    }

    public async Task<IActionResult> Index()
    {

        var result = await _analyticService.GetDataAsync();

        if (!result.Success)   
        {
            _logger.LogWarning("Home page loaded without analytics data");
            return View(new AnalyticViewModel());
        }

        return View(result.Value);   
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
