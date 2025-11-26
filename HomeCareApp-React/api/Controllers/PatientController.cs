using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HomeCareApp.Models;
using AppUser = HomeCareApp.Models.User;

namespace HomeCareApp.Controllers
{
    [Authorize(Roles = "Personnel,Admin")]
    public class PatientController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PatientController> _logger;

        public PatientController(UserManager<AppUser> userManager, ILogger<PatientController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // -----------------------------
        // LIST ALL PATIENTS
        // -----------------------------
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("PatientController.Index called by {User}", User.Identity?.Name);

                var patients = await _userManager.GetUsersInRoleAsync("Patient");

                _logger.LogInformation("Loaded {Count} patients", patients.Count);

                return View(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading patient list");
                TempData["Error"] = "Could not load patients.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
