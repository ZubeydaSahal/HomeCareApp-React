using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeCareApp.Service.User;
using HomeCareApp.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using HomeCareApp.Models;

namespace HomeCareApp.Controllers.User;

public class RegisterController : Controller
{
    private readonly IUserService _userService;
    private readonly SignInManager<HomeCareApp.Models.User> _signInManager;
    private readonly UserManager<HomeCareApp.Models.User> _userManager;

    public RegisterController(IUserService userService, UserManager<HomeCareApp.Models.User> userManager, SignInManager<HomeCareApp.Models.User> signInManager)
    {
        _userService = userService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET /User/Register
    [HttpGet("/User/Register")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View("~/Views/User/Register.cshtml");
    }

    // POST /User/Register
    [HttpPost("/User/Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View("~/Views/User/Register.cshtml", model);

        var result = await _userService.RegisterAsync(model);
        if (result.Succeeded)
        {
            // sign in the newly registered user
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                // redirect based on role
                var role = (model.Role ?? "Patient").ToLowerInvariant();
                if (role == "personnel") return RedirectToAction("Personnel", "Dashboard");
                return RedirectToAction("Patient", "Dashboard");
            }

            // fallback to login if sign-in failed
            return RedirectToAction("Login", "Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("~/Views/User/Register.cshtml", model);
    }
}