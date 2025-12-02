using System.Security.Claims;
using HomeCareApp.DAL;
using HomeCareApp.DTOs;
using HomeCareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppUser = HomeCareApp.Models.User;

namespace HomeCareApp.Controllers;

[ApiController]
[Route("api/availability")]
[Authorize]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<AvailabilityController> _logger; // Logger

    public AvailabilityController(
        IAvailabilityRepository availabilityRepository,
        UserManager<AppUser> userManager,
        ILogger<AvailabilityController> logger)
    {
        _availabilityRepository = availabilityRepository;
        _userManager = userManager;
        _logger = logger; // Injected logger
    }

    // --------------------------------------------------------------------
    // GET: api/availability/list
    // --------------------------------------------------------------------
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> List()
    {
        try
        {
            _logger.LogInformation("Listing availability for user {User}", User.Identity?.Name); // Info log

            var list = await _availabilityRepository.GetAllAsync() ?? new List<Availability>();

            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");
            var isPersonnel = User.IsInRole("Personnel");
            var isPatient = User.IsInRole("Patient");

            if (isPersonnel && !isAdmin && !string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("Filtering for personnel user {UserId}", userId); // Info log
                list = list.Where(a => a.PersonnelId == userId).ToList();
            }

            if (isPatient)
            {
                _logger.LogInformation("Filtering free slots for patient"); // Info log
                list = list.Where(a => a.Appointment == null).ToList();
            }

            var result = list.Select(a => new AvailabilityDto
            {
                Id = a.Id,
                PersonnelId = a.PersonnelId,
                PersonnelName = a.Personnel?.FullName,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Notes = a.Notes,
                AppointmentId = a.Appointment?.Id
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing availability"); // Error log
            return StatusCode(500, "Internal server error");
        }
    }


    // --------------------------------------------------------------------
    // GET: api/availability/5
    // --------------------------------------------------------------------
    [HttpGet("{id}")]
    public async Task<ActionResult<AvailabilityDto>> Get(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving availability {Id}", id); // Info log

            var a = await _availabilityRepository.GetByIdAsync(id);
            if (a == null)
            {
                _logger.LogWarning("Availability {Id} not found", id); // Warning log
                return NotFound();
            }

            var dto = new AvailabilityDto
            {
                Id = a.Id,
                PersonnelId = a.PersonnelId,
                PersonnelName = a.Personnel?.FullName,
                Date = a.Date,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Notes = a.Notes,
                AppointmentId = a.Appointment?.Id
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving availability {Id}", id); // Error log
            return StatusCode(500, "Internal server error");
        }
    }


    // --------------------------------------------------------------------
    // POST: api/availability/create
    // --------------------------------------------------------------------
    [HttpPost("create")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Create([FromBody] AvailabilityCreateDto dto)
    {
        try
        {
            _logger.LogInformation("Creating availability for {User}", User.Identity?.Name); // Info log

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for create availability"); // Warning log
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UserId missing in token on create"); // Warning log
                return Unauthorized("Could not read user id from token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Personnel not found: {UserId}", userId); // Warning log
                return BadRequest($"No Identity user found with Id={userId}");
            }

            var availability = new Availability
            {
                PersonnelId = user.Id,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Notes = dto.Notes
            };

            await _availabilityRepository.AddAsync(availability);

            _logger.LogInformation("Availability created with Id {Id}", availability.Id); // Info log

            var resultDto = new AvailabilityDto
            {
                Id = availability.Id,
                PersonnelId = availability.PersonnelId,
                PersonnelName = user.FullName,
                Date = availability.Date,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                Notes = availability.Notes
            };

            return CreatedAtAction(nameof(Get), new { id = availability.Id }, resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating availability"); // Error log
            return StatusCode(500, "Internal server error");
        }
    }


    // --------------------------------------------------------------------
    // PUT: api/availability/update/5
    // --------------------------------------------------------------------
    [HttpPut("update/{id}")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Update(int id, [FromBody] AvailabilityCreateDto dto)
    {
        try
        {
            _logger.LogInformation("Updating availability {Id}", id); // Info log

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for update {Id}", id); // Warning log
                return BadRequest(ModelState);
            }

            var entity = await _availabilityRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Availability {Id} not found for update", id); // Warning log
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Missing userId for update availability"); // Warning log
                return Unauthorized("Could not read user id from token.");
            }

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && entity.PersonnelId != userId)
            {
                _logger.LogWarning("Forbidden update attempt by {UserId}", userId); // Warning log
                return Forbid();
            }

            entity.Date = dto.Date;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Notes = dto.Notes;

            await _availabilityRepository.UpdateAsync(entity);

            _logger.LogInformation("Availability {Id} updated successfully", id); // Info log

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability {Id}", id); // Error log
            return StatusCode(500, "Internal server error");
        }
    }


    // --------------------------------------------------------------------
    // DELETE: api/availability/delete/5
    // --------------------------------------------------------------------
    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Deleting availability {Id}", id); // Info log

            var entity = await _availabilityRepository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("Availability {Id} not found for delete", id); // Warning log
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Missing userId for delete availability"); // Warning log
                return Unauthorized("Could not read user id from token.");
            }

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && entity.PersonnelId != userId)
            {
                _logger.LogWarning("Forbidden delete attempt by {UserId}", userId); // Warning log
                return Forbid();
            }

            await _availabilityRepository.DeleteAsync(id);

            _logger.LogInformation("Availability {Id} deleted", id); // Info log

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting availability {Id}", id); // Error log
            return StatusCode(500, "Internal server error");
        }
    }


    private string? GetCurrentUserId()
    {
        try
        {
            var nameIdClaims = User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .ToList();

            if (!nameIdClaims.Any())
            {
                _logger.LogWarning("No NameIdentifier claim on user"); // Warning log
                return null;
            }

            return nameIdClaims.Last().Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting user id claim"); // Error log
            return null;
        }
    }
}
