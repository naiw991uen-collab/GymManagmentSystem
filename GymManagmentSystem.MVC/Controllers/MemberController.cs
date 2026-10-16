using FluentValidation;
using GymManagmentSystem.BLL.Services.Members.Classes;
using GymManagmentSystem.BLL.Services.Members.Interfaces;
using GymManagmentSystem.BLL.Services.Members.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentSystem.PL.Controllers;
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
     private readonly IValidator<CreateMemberViewModel> _validator;
    public MembersController(IMemberService memberService, IValidator<CreateMemberViewModel> validator)
    {
        _memberService = memberService;
        _validator = validator;
    }
    public async Task<IActionResult> Index()
    {
        var members = await _memberService.GetMembersAsyn();
        return View(members);
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

        if (result)
        {
            TempData["SuccessMessage"] = "Member Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = "Member Failed To Create";
            return View(nameof(Create), model);
        }
    }

    public async Task<IActionResult> MemberDetails(int? Id)
    {
        var member = await _memberService.GetMemberAsync(Id);

        if(member == null)
        {
            TempData["ErrorMessage"] = "Member Not Found";
            return RedirectToAction(nameof(Index));
        }

        return View(member);
    }

    public async Task<IActionResult> HealthRecordDetails(int? Id)
    {
        var model = await _memberService.GetHealthRecordAsync(Id);

        if (model == null)
        {
            TempData["ErrorMessage"] = "HealthRecord Not Found";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    public async Task<IActionResult> EditMember(int? Id)
    {
        var member = await _memberService.GetMemberToUpdateAsync(Id);

        if (member == null)
        {
            TempData["ErrorMessage"] = "Member Not Found";
            return RedirectToAction(nameof(Index));
        }
        return View(member);
    }

    [HttpPost]
    public async Task<IActionResult> EditMember(MemberToUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(nameof(Create), model);

        var result = await _memberService.UpdateMemberAsync(model);

        if (result)
        {
            TempData["SuccessMessage"] = "Member Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = "Member Failed To Update";
            return View(nameof(Create), model);
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

        if (result)
        {
            TempData["SuccessMessage"] = "Member Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = "Member Failed To Delete";
            return View(Id);
        }
    }
}
