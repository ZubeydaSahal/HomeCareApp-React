using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeCareApp.ViewModels.User;
using HomeCareApp.DAL;
namespace HomeCareApp.Controllers.User
{
    public class RegisterController : Controller
    {
        private readonly IUserRepository _userRepository;

        public RegisterController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            if (!ModelState.IsValid)
            {
                return View("~/Views/User/Register.cshtml", model);
            }

            // Sjekk om bruker allerede finnes
            var existing = await _userRepository.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                return View("~/Views/User/Register.cshtml", model);
            }

            // Opprett ny bruker
            var user = new HomeCareApp.Models.User
            {
                UserName = model.Email,
                Email = model.Email,
                // Hvis RegisterViewModel har f.eks. FullName, PhoneNumber osv:
                // FullName = model.FullName,
                // PhoneNumber = model.PhoneNumber
            };

            var createResult = await _userRepository.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("~/Views/User/Register.cshtml", model);
            }

            // Legg bruker i rolle (default: Patient)
            var role = (model.Role ?? "Patient").Trim();
            var roleResult = await _userRepository.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("~/Views/User/Register.cshtml", model);
            }

            // Logg inn ny bruker med passordet de nettopp valgte
            var signInResult = await _userRepository.PasswordSignInAsync(user, model.Password);
            if (!signInResult.Succeeded)
            {
                // Fallback: gå til login-side hvis automatisk innlogging feiler
                return RedirectToAction("Login", "Login");
            }

            // Redirect basert på rolle
            var normalizedRole = role.ToLowerInvariant();
            if (normalizedRole == "personnel")
            {
                return RedirectToAction("Personnel", "Dashboard");
            }

            // Default: Patient-dashboard
            return RedirectToAction("Patient", "Dashboard");
        }
    }
}
