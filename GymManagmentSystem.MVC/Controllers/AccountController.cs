using GymManagmentSystem.BLL.Services.Accounut.ViewModel;
using GymManagmentSystem.BLL.Services.Analytics.Interfaces;
using GymManagmentSystem.DAL.Applications;
using GymManagmentSystem.MVC.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentSystem.PL.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IAnalyticService _analyticService;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, IAnalyticService analyticService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _analyticService = analyticService;
    }

    public async Task<IActionResult> Login()
    {
        await LoadStats();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        await LoadStats();

        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            ModelState.AddModelError(nameof(model.Email), "Invalid Email");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            _logger.LogInformation("Login is successful");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning("User locked out");
            ModelState.AddModelError(string.Empty, "This account is locked");
            return View(model);
        }
        else
        {
            _logger.LogWarning("Password is incorrect");
            ModelState.AddModelError(nameof(model.Password), "Invalid Password");
            return View(model);
        }
    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task LoadStats()
    {
        var result = await _analyticService.GetDataAsync();
        var data = result.Success ? result.Value : null;

        ViewBag.MembersCount = data?.TotalMembers ?? 0;
        ViewBag.TrainersCount = data?.TotalTrainers ?? 0;
    }
}
