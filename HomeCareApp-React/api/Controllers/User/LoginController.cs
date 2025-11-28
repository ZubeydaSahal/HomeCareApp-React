using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeCareApp.ViewModels.User;
using HomeCareApp.DAL;

namespace HomeCareApp.Controllers.User
{
    public class LoginController : Controller
    {
        private readonly IUserRepository _userRepository;

        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET /User/Login
        [HttpGet("/User/Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Go", "Home");
            }

            return View("~/Views/User/Login.cshtml");
        }

        // POST /User/Login
        [HttpPost("/User/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/User/Login.cshtml", model);
            }

            // Finn bruker via repo
            var user = await _userRepository.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View("~/Views/User/Login.cshtml", model);
            }

            // Forsøk å logge inn
            var result = await _userRepository.PasswordSignInAsync(user, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Go", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View("~/Views/User/Login.cshtml", model);
        }

        // POST /User/Logout
        [HttpPost("/User/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userRepository.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
