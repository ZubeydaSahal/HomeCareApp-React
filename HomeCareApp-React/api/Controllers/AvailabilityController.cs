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

    public AvailabilityController(
        IAvailabilityRepository availabilityRepository,
        UserManager<AppUser> userManager)
    {
        _availabilityRepository = availabilityRepository;
        _userManager = userManager;
    }

    // --------------------------------------------------------------------
    // GET: api/availability/list
    // --------------------------------------------------------------------
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> List()
    {
        var list = await _availabilityRepository.GetAllAsync() ?? new List<Availability>();

        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var isPersonnel = User.IsInRole("Personnel");
        var isPatient = User.IsInRole("Patient");

        // Pleier (ikke admin) → bare egne slots
        if (isPersonnel && !isAdmin && !string.IsNullOrEmpty(userId))
        {
            list = list.Where(a => a.PersonnelId == userId).ToList();
        }

        // Pasient → bare ledige slots (ikke booket)
        if (isPatient)
        {
            list = list
                .Where(a => a.Appointment == null)
                .ToList();
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


    // --------------------------------------------------------------------
    // GET: api/availability/5
    // --------------------------------------------------------------------
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AvailabilityDto>> Get(int id)
    {
        var a = await _availabilityRepository.GetByIdAsync(id);
        if (a == null) return NotFound();

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

    // --------------------------------------------------------------------
    // POST: api/availability/create
    // Bruker innlogget bruker (fra JWT) som Personnel

    [HttpPost("create")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Create([FromBody] AvailabilityCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Could not read user id from token.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
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


    // --------------------------------------------------------------------
    // PUT: api/availability/update/5
    // Kun eier selv eller Admin får lov å oppdatere
    // --------------------------------------------------------------------
    [HttpPut("update/{id:int}")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Update(int id, [FromBody] AvailabilityCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var entity = await _availabilityRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Could not read user id from token.");
        }

        var isAdmin = User.IsInRole("Admin");

        // Ikke admin → må være eier av availability
        if (!isAdmin && entity.PersonnelId != userId)
        {
            return Forbid(); // 403
        }

        entity.Date = dto.Date;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.Notes = dto.Notes;

        await _availabilityRepository.UpdateAsync(entity);
        return NoContent();
    }


    // --------------------------------------------------------------------
    // DELETE: api/availability/delete/5
    // Kun eier selv eller Admin får lov å slette
    // --------------------------------------------------------------------
    [HttpDelete("delete/{id:int}")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _availabilityRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Could not read user id from token.");
        }

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && entity.PersonnelId != userId)
        {
            return Forbid();
        }

        await _availabilityRepository.DeleteAsync(id);
        return NoContent();
    }
    private string? GetCurrentUserId()
    {
        var nameIdClaims = User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .ToList();

        if (!nameIdClaims.Any())
        {
            var claimsDump = string.Join(", ",
                User.Claims.Select(c => $"{c.Type}={c.Value}"));
            Console.WriteLine($"[AvailabilityApiController] No NameIdentifier claim. Claims: {claimsDump}");
            return null;
        }


        return nameIdClaims.Last().Value;
    }

}
