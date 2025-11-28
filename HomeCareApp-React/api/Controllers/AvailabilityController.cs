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
public class AvailabilityApiController : ControllerBase
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly UserManager<AppUser> _userManager;  

    public AvailabilityApiController(
        IAvailabilityRepository availabilityRepository,
        UserManager<AppUser> userManager)               
    {
        _availabilityRepository = availabilityRepository;
        _userManager = userManager;
    }

    // GET: api/availability/list
    [HttpGet("list")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> List()
    {
        var list = await _availabilityRepository.GetAllAsync();

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

    // GET: api/availability/5
    [HttpGet("{id:int}")]
    [AllowAnonymous]
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

    //[Authorize(Roles = "Personnel,Admin")]
    // POST: api/availability/create
[HttpPost("create")]
[AllowAnonymous] // mens du tester UTEN auth
public async Task<ActionResult> Create([FromBody] AvailabilityCreateDto dto)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    // Hent sykepleieren fra databasen
    var nurse = await _userManager.FindByEmailAsync("nurse@homecare.local");
    if (nurse == null)
    {
        return StatusCode(500, "Seed user 'nurse@homecare.local' not found.");
    }

    var availability = new Availability
    {
        PersonnelId = nurse.Id,   // <- nå peker FK på en ekte rad
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
        Date = availability.Date,
        StartTime = availability.StartTime,
        EndTime = availability.EndTime,
        Notes = availability.Notes
    };

    return CreatedAtAction(nameof(Get), new { id = availability.Id }, resultDto);
}



    // PUT: api/availability/update/5
    [HttpPut("update/{id:int}")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Update(int id, [FromBody] AvailabilityCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var entity = await _availabilityRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        // ev. sjekk at innlogget bruker eier denne availabilityen

        entity.Date = dto.Date;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.Notes = dto.Notes;

        await _availabilityRepository.UpdateAsync(entity);
        return NoContent();
    }

    // DELETE: api/availability/delete/5
    [HttpDelete("delete/{id:int}")]
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _availabilityRepository.GetByIdAsync(id);
        if (entity == null) return NotFound();

        await _availabilityRepository.DeleteAsync(id);
        return NoContent();
    }
}
