using GymManagmentSystem.BLL.Services.Plansa.Interfaces;
using GymManagmentSystem.BLL.Services.Plansa.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymManagmentSystem.MVC.Controllers;

[Authorize]
public class PlansController : Controller
{
    private IRepository<Plan> _planRepository;
    private readonly IPlanService _planService;
    private readonly IDashboardNotifier _dashboardNotifier;
    public PlansController(IRepository<Plan> planRepository, IPlanService planService, IDashboardNotifier dashboardNotifier)
    {
        _planRepository = planRepository;
        _planService = planService;
        _dashboardNotifier = dashboardNotifier;
    }
    private async Task<IActionResult> ReturnViewWithNameAsync(EditPlanViewModel model)
    {
        var original = await _planService.GetPlanAsync(model.Id);
        model.PlanName = original.Success ? original.Value!.PlanName : string.Empty;
        return View(model);
    }
    public async Task<IActionResult> Index()
    {
        var plans = await _planRepository.GetAllAsync();
        return View(plans);
    }
    public async Task<IActionResult> Details(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);

        if (plan is null)
            return RedirectToAction(nameof(Index));
        else
            return View(plan);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _planService.GetPlanAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditPlanViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _planService.UpdatePlanAsync(model);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Plan Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
            return await ReturnViewWithNameAsync(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Active(int? id, bool wasActive)
    {
        var result = await _planService.DeactivateMemberAsync(id);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = wasActive
                ? "Plan Deactivated Successfully"
                : "Plan Activated Successfully";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }

        return RedirectToAction(nameof(Index));
    }
}
