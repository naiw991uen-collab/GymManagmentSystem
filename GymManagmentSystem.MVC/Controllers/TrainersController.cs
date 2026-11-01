using FluentValidation;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.BLL.Services.Trainers.Interfaces;
using GymManagmentSystem.BLL.Services.Trainers.ViewModel;
using GymManagmentSystem.DAL.Models;
using GymManagmentSystem.MVC.Controllers;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentSystem.PL.Controllers;

[Authorize]
public class TrainersController : Controller
{
    private readonly ITrainerService _trainerService;
    private readonly IValidator<CreateMemberViewModel> _validator;
    private readonly IDashboardNotifier _dashboardNotifier;
    public TrainersController(ITrainerService trainerService, IValidator<CreateMemberViewModel> validator, IDashboardNotifier dashboardNotifier)
    {
        _trainerService = trainerService;
        _validator = validator;
        _dashboardNotifier = dashboardNotifier;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _trainerService.GetTrainersAsync();
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
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTrainerViewModel model)
    {
        if (!ModelState.IsValid)
            return View(nameof(Create), model);

        var result = await _trainerService.AddTrainerAsyn(model);

        if(result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Trainer Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
            return View(nameof(Create), model);
        }
    }

    public async Task<IActionResult> Details(int? Id)
    {
        var result = await _trainerService.GetTrainerAsync(Id);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    public async Task<IActionResult> Edit(int? Id)
    {
        var result = await _trainerService.GetTrainerToUpdateAsync(Id);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
   }

    [HttpPost]
    public async Task<IActionResult> Edit(TrainerToUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _trainerService.UpdateTrainerAsync(model);

        if (result.Success)
        {
            TempData["SuccessMessage"] = "Trainer Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
            return View(model);
        }
    }
     
    public async Task<IActionResult> Delete(int? Id)
    {
        return View(Id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int? Id)
    {
        var result = await _trainerService.DeleteTrainerAsync(Id);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;

            return View(Id);
        }
    }
}
