using FluentValidation;
using GymManagmentSystem.BLL.Services.Attachment.Interface;
using GymManagmentSystem.BLL.Services.Members.Classes;
using GymManagmentSystem.BLL.Services.Members.Interfaces;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using GymManagmentSystem.MVC.Controllers;
using GymManagmentSystem.PL.Hubs.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentSystem.PL.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
     private readonly IValidator<CreateMemberViewModel> _validator;
    private readonly IAttachmentService _attachmentService;
    private readonly IDashboardNotifier _dashboardNotifier;
    public MembersController(IMemberService memberService, IValidator<CreateMemberViewModel> validator, IDashboardNotifier dashboardNotifier, IAttachmentService attachmentService)
    {
        _memberService = memberService;
        _validator = validator;
        _dashboardNotifier = dashboardNotifier;
        _attachmentService = attachmentService;
    }
    public async Task<IActionResult> Index()
    {
        var result = await _memberService.GetMembersAsyn();

        if(result.Success)
        {
            return View(result.Value);
        }
        else
        {
            return RedirectToAction(nameof(Index), nameof(HomeController));
        }
        
    }

    //[HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMember(CreateMemberViewModel model)
    {
        if (!ModelState.IsValid)
            return View(nameof(Create), model);

        var result = await _memberService.AddMemberAsync(model);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Member Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
            return View(nameof(Create), model);
        }
    }

    public async Task<IActionResult> Picture(int Id)
    {
        var result = await _memberService.GetMemberAsync(Id);

        if (!result.Success || string.IsNullOrWhiteSpace(result.Value?.Photo))
            return NotFound();

        var file = _attachmentService.GetFile(result.Value.Photo, "MemberPhotos");

        if (!file.Success || file.Value == null)
            return NotFound();

        return File(file.Value.Stream, file.Value.ContentType);
    }
    public async Task<IActionResult> MemberDetails(int? Id)
    {
        var result = await _memberService.GetMemberAsync(Id);

        if(!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    public async Task<IActionResult> HealthRecordDetails(int? Id)
    {
        var result = await _memberService.GetHealthRecordAsync(Id);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    public async Task<IActionResult> EditMember(int? Id)
    {
        var result = await _memberService.GetMemberToUpdateAsync(Id);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> EditMember(MemberToUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _memberService.UpdateMemberAsync(model);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Member Updated Successfully";
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
        var result = await _memberService.DeleteMemberAsync(Id);

        if (result.Success)
        {
            await _dashboardNotifier.NotifyStatsChangedAsync();
            TempData["SuccessMessage"] = "Member Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;

            return View(Id);
        }
    }
}
