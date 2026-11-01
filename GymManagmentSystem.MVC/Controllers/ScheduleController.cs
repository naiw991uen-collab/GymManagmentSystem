using FluentValidation;
using GymManagmentSystem.BLL.Services.Booking.Interfaces;
using GymManagmentSystem.BLL.Services.Booking.ViewModel;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.BLL.Services.MemberShips.Interfaces;
using GymManagmentSystem.MVC.Controllers;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagmentSystem.PL.Controllers;

[Authorize]
public class ScheduleController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IValidator<CreateMemberViewModel> _validator;
    private readonly IDashboardNotifier _dashboardNotifier;

    public ScheduleController(IBookingService bookingService, IValidator<CreateMemberViewModel> validator, IDashboardNotifier dashboardNotifier)
    {
        _bookingService = bookingService;
        _validator = validator;
        _dashboardNotifier = dashboardNotifier;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetAllSessionsAsync(ct: cancellationToken);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction("Index", "Home");
        }
        if (!result.Value.Any())
        {
            TempData["ErrorMessage"] = "No upcoming sessions available. Please create a session first.";
            return RedirectToAction("Index", "Sessions");
        }
        return View(result.Value);
    }

    public async Task<IActionResult> GetMembersForUpcomingSession(int id, CancellationToken ct)
    {
        var members = await _bookingService.GetMemberForUpCommingSessionByIdAsync(id, ct);
        return View(members);
    }

    public async Task<IActionResult> GetMembersForOngoingSession(int id, CancellationToken ct)
    {
        var members = await _bookingService.GetMemberForOnGoingSessionByIdAsync(id, ct);
        return View(members);
    }

    public async Task<IActionResult> Create(int Id , CancellationToken ct)
    {
        var members = await _bookingService.GetMemberForDropdownViewModelAsync(Id, ct);

        ViewBag.Members = new SelectList(members, "Id", "Name");

        ViewBag.SessionId = Id;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingViewModel model , CancellationToken ct)
    {
        var result = await _bookingService.CreateNewBookingAsync(model, ct);

        if (result.Success)
            await _dashboardNotifier.NotifyStatsChangedAsync();

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Booking Created Successfully" : result.Error;

        return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = model.SessionId });
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int memberId , int sessionId , CancellationToken ct)
    {
        var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);

        if (result.Success)
            await _dashboardNotifier.NotifyStatsChangedAsync();

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Booking Canceled Successfully" : result.Error;

        return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });

    }

    [HttpPost]
    public async Task<IActionResult> Attended(int memberId , int sessionId , CancellationToken ct)
    {
        var result = await _bookingService.MarkAsAttend(memberId, sessionId, ct);

        if (result.Success)
            await _dashboardNotifier.NotifyStatsChangedAsync();

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage "] = result.Success ? "Attendeance Recorded" : result.Error;

        return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });
    }
}
