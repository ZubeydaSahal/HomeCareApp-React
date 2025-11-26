using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeCareApp.Service.User;
using HomeCareApp.ViewModels.User;

namespace HomeCareApp.Controllers.User;

public class LoginController : Controller
{
    private readonly IUserService _userService;

    public LoginController(IUserService userService)
    {
        _userService = userService;
    }

    // GET /User/Login
    [HttpGet("/User/Login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Go", "Home");
        return View("~/Views/User/Login.cshtml");
    }

    // POST /User/Login
    [HttpPost("/User/Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View("~/Views/User/Login.cshtml", model);
        
        var result = await _userService.LoginAsync(model);
        if (result.Succeeded) return RedirectToAction("Go", "Home");
        
        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return View("~/Views/User/Login.cshtml", model);
    }

    // POST /User/Logout
    [HttpPost("/User/Logout")]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}