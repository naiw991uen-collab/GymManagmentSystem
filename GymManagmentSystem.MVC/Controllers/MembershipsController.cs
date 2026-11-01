using FluentValidation;
using GymManagmentSystem.BLL.Services.Members.Interfaces;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.BLL.Services.MemberShips.Interfaces;
using GymManagmentSystem.BLL.Services.MemberShips.ViewModel;
using GymManagmentSystem.MVC.Controllers;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagmentSystem.PL.Controllers;

[Authorize]
public class MembershipsController : Controller
{
    private readonly IMemberShipService _membershipService;
    private readonly IValidator<CreateMemberViewModel> _validator;
    private readonly IDashboardNotifier _dashboardNotifier;

    public MembershipsController(IMemberShipService membershipService, IValidator<CreateMemberViewModel> validator, IDashboardNotifier dashboardNotifier)
    {
        _membershipService = membershipService;
        _validator = validator;
        _dashboardNotifier = dashboardNotifier;
    }


    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await _membershipService.GetAllMembershipAsync(cancellationToken));
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await PopularDropDownAsync(cancellationToken);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberShipViewModel model , CancellationToken ct)
    {

        if (!ModelState.IsValid)
        {
           await PopularDropDownAsync(ct);

           return View(model);
        }

        var result = await _membershipService.CreateMemberShipAsync(model, ct);

        if(result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Membership Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = result.Error;

        await PopularDropDownAsync(ct);
         
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id , CancellationToken ct)
    {
        var result = await _membershipService.DeleteActiceMemberShipAsync(id);

        if (result.Success)
            await _dashboardNotifier.NotifyStatsChangedAsync();

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Membership Cancel" : result.Error;

        return RedirectToAction(nameof(Index));
    }
    private async Task PopularDropDownAsync(CancellationToken ct)
    {
        ViewBag.Plans = new SelectList(await _membershipService.GetAllPlanSelectListAsync(ct), "Id", "Name");
        ViewBag.Members = new SelectList(await _membershipService.GetAllMemberSelectListAsync(ct), "Id", "Name");
    }
}
