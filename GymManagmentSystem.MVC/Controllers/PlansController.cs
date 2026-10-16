using GymManagmentSystem.BLL.Services.Plansa.Interfaces;
using GymManagmentSystem.BLL.Services.Plansa.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace GymManagmentSystem.MVC.Controllers;
public class PlansController : Controller
{
    private async Task<IActionResult> ReturnViewWithNameAsync(EditPlanViewModel model)
    {
        var original = await _planService.GetPlanAsync(model.Id);
        model.PlanName = original?.PlanName ?? string.Empty;
        return View(model);
    }
    private IRepository<Plan> _planRepository;
    private readonly IPlanService _planService ;
    public PlansController(IRepository<Plan> planRepository, IPlanService planService)
    {
        _planRepository = planRepository;
        _planService = planService;
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
        var plan = await _planService.GetPlanAsync(id);
        if (plan == null)
        {
            TempData["ErrorMessage"] = "Plan Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View(plan);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditPlanViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _planService.UpdatePlanAsync(model);

        if (result)
        {
            TempData["SuccessMessage"] = "Plan Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Plan Failed To Update");
        return await ReturnViewWithNameAsync(model);
    }

    [HttpPost]
    public async Task<IActionResult> Active(int? id, bool wasActive)
    {
        var result = await _planService.DeactivateMemberAsync(id);

        if (result)
        {
            TempData["SuccessMessage"] = wasActive
                ? "Plan Deactivated Successfully"
                : "Plan Activated Successfully";
        }
        else
        {
            TempData["ErrorMessage"] = wasActive
                ? "Plan Failed To Deactivate"
                : "Plan Failed To Activate";
        }

        return RedirectToAction(nameof(Index));
    }
}
