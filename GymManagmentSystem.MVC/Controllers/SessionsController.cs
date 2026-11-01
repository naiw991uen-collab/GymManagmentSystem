using GymManagmentSystem.BLL.Services.Sessions.Interfaces;
using GymManagmentSystem.BLL.Services.Sessions.ViewModel;
using GymManagmentSystem.MVC.Controllers;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagmentSystem.PL.Controllers;

[Authorize]
public class SessionsController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IDashboardNotifier _dashboardNotifier;

    public SessionsController(ISessionService sessionService, IDashboardNotifier dashboardNotifier)
    {
        _sessionService = sessionService;
        _dashboardNotifier = dashboardNotifier;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _sessionService.GetAllSessionsAsync();

        if (result.Success)
        {
            return View(result.Value);
        }
        else
        {
            return RedirectToAction(nameof(Index), nameof(HomeController));
        }
    }

    public async Task<IActionResult> Create()
    {
        await LoadTrainerAndCategorisForSelectedList();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSessionViewModel model)
    {
        var result = await _sessionService.CreateSessionAsync(model);

        if(result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Session Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            await LoadTrainerAndCategorisForSelectedList();

            TempData["ErrorMessage"] = result.Error;
            return View(model);
        }

    }

    public async Task<IActionResult> Details(int? Id)
    {
        var result = await _sessionService.GetSessionDetailsAsync(Id);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    public async Task<IActionResult> Edit(int? Id)
    {
        var result = await _sessionService.GetSessionToUpdateAsync(Id);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        await LoadTrainerAndCategorisForSelectedList();

        return View(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SessionToUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadTrainerAndCategorisForSelectedList();

            return View(model);
        }

        var result = await _sessionService.UpdateSessionAsync(model);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Trainer Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = result.Error;

        await LoadTrainerAndCategorisForSelectedList();

        return View(model);
    }

    public async Task<IActionResult> Delete(int? Id)
    {
        return View(Id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int? Id)
    {
        var result = await _sessionService.DeleteSessionAsync(Id);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Session Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;

            return View(Id);
        }
    }

    #region Helper Methods
    private async Task LoadTrainerAndCategorisForSelectedList()
    {
        var trainers = await _sessionService.GetTrainersForDropDownAsync();
        var categories = await _sessionService.GetCategoriesForDropDownAsync();

        ViewBag.Trainers = new SelectList(trainers.Value, "Id", "Name");
        ViewBag.Categories = new SelectList(categories.Value, "Id", "CategoryName");
    }

    #endregion
}

