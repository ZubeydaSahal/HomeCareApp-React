using HomeCareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HomeCareApp.DTOs;
namespace HomeCareApp.Controllers;


[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase  // inherits from ControllerBase for API controllers
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<User> userManager,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    // -------------------------------------------------
    // GET: api/admin/patients
    // -> Admin sees all patients
    // -------------------------------------------------
    [HttpGet("patients")]
    public async Task<ActionResult<IEnumerable<AdminDto>>> GetPatients()
    {
        _logger.LogInformation("[AdminController] GetPatients called by {User}", User.Identity?.Name);

        var patients = await _userManager.GetUsersInRoleAsync("Patient");

        var dto = patients.Select(p => new AdminDto
        {
            Id = p.Id,
            FullName = p.FullName ?? "(No name)",
            Email = p.Email,
            Role = "Patient"
        });

        return Ok(dto);
    }

    // -------------------------------------------------
    // GET: api/admin/personnel
    // -> Admin sees all personnel
    // -------------------------------------------------
    [HttpGet("personnel")]
    public async Task<ActionResult<IEnumerable<AdminDto>>> GetPersonnel()
    {
        _logger.LogInformation("[AdminController] GetPersonnel called by {User}", User.Identity?.Name);

        var personnel = await _userManager.GetUsersInRoleAsync("Personnel");

        var dto = personnel.Select(p => new AdminDto
        {
            Id = p.Id,
            FullName = p.FullName ?? "(No name)",
            Email = p.Email,
            Role = "Personnel"
        });

        return Ok(dto);
    }


}

