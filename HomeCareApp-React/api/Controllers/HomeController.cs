using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HomeCareApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace HomeCareApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // -----------------------------------------
    // INDEX (Landing Page)
    // -----------------------------------------
    [AllowAnonymous]
    public IActionResult Index()
    {
        try
        {
            _logger.LogInformation("Home.Index accessed by {User}", User.Identity?.Name ?? "Anonymous");

            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("User {User} authenticated. Redirecting to Go()", User.Identity?.Name);
                return RedirectToAction("Go");
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Home.Index");
            TempData["Error"] = "Unexpected error occurred.";
            return RedirectToAction("Error");
        }
    }

    // Login handled by AuthController
    [Authorize]
    public IActionResult Go()
    {
        try
        {
            _logger.LogInformation("Home.Go accessed by {User}", User.Identity?.Name);

            // Prefer Patient dashboard when both roles are accidentally assigned
            if (User.IsInRole("Patient"))
            {
                _logger.LogInformation("Routing {User} to Patient Dashboard", User.Identity?.Name);
                return RedirectToAction("Patient", "Dashboard");
            }

            if (User.IsInRole("Personnel"))
            {
                _logger.LogInformation("Routing {User} to Personnel Dashboard", User.Identity?.Name);
                return RedirectToAction("Personnel", "Dashboard");
            }

            _logger.LogWarning("User {User} has no valid role. Redirecting to Index", User.Identity?.Name);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Home.Go");
            TempData["Error"] = "Unexpected error occurred.";
            return RedirectToAction("Error");
        }
    }

    // -----------------------------------------
    // PRIVACY
    // -----------------------------------------
    public IActionResult Privacy()
    {
        _logger.LogInformation("Home.Privacy accessed by {User}", User.Identity?.Name);
        return View();
    }

    // -----------------------------------------
    // ERROR
    // -----------------------------------------
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        _logger.LogError("Home.Error triggered. RequestId={RequestId}", requestId);

        return View(new ErrorViewModel { RequestId = requestId });
    }
}
