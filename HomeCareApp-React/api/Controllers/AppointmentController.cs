using HomeCareApp.DAL;
using HomeCareApp.DTOs;
using HomeCareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppUser = HomeCareApp.Models.User;

namespace HomeCareApp.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize(Roles = "Personnel,Patient,Admin")]
public class AppointmentApiController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly UserManager<AppUser> _userManager;

    public AppointmentApiController(
        IAppointmentRepository appointmentRepository,
        IAvailabilityRepository availabilityRepository,
        UserManager<AppUser> userManager)
    {
        _appointmentRepository = appointmentRepository;
        _availabilityRepository = availabilityRepository;
        _userManager = userManager;
    }

    // GET: api/appointments/list
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> List()
    {
        var all = await _appointmentRepository.GetAllAsync() ?? new List<Appointment>();

        var dtos = all.Select(a => new AppointmentDto
        {
            Id = a.Id,
            AvailabilityId = a.AvailabilityId,
            ClientId = a.ClientId,
            ClientName = a.Client?.FullName,
            PersonnelId = a.Availability?.PersonnelId,
            PersonnelName = a.Availability?.Personnel?.FullName,
            //Date = a.Availability?.Date ?? default,  // DateOnly

    
            // Hvis Appointment.StartTime er TimeSpan, bruk heller:
            StartTime = TimeOnly.FromTimeSpan(a.StartTime),
            EndTime   = TimeOnly.FromTimeSpan(a.EndTime),

            TaskDescription = a.TaskDescription,
            Status = a.Status
        });

        return Ok(dtos);
    }

    // GET: api/appointments/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDto>> Get(int id)
    {
        var a = await _appointmentRepository.GetByIdAsync(id);
        if (a == null) return NotFound();

        var dto = new AppointmentDto
        {
            Id = a.Id,
            AvailabilityId = a.AvailabilityId,
            ClientId = a.ClientId,
            ClientName = a.Client?.FullName,
            PersonnelId = a.Availability?.PersonnelId,
            PersonnelName = a.Availability?.Personnel?.FullName,
            //Date = a.Availability?.Date ?? default,

            // Samme kommentar som over ang. TimeOnly vs TimeSpan
            StartTime = TimeOnly.FromTimeSpan(a.StartTime),
            EndTime   = TimeOnly.FromTimeSpan(a.EndTime),

            TaskDescription = a.TaskDescription,
            Status = a.Status
        };

        return Ok(dto);
    }

    // POST: api/appointments/create
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] AppointmentCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var slot = await _availabilityRepository.GetByIdAsync(dto.AvailabilityId);
        if (slot == null) return BadRequest("Selected availability does not exist.");
        if (slot.Appointment != null) return BadRequest("This time slot is already booked.");

        // Hvem er klient?
        string? clientId;

        if (User.IsInRole("Patient"))
        {
            clientId = _userManager.GetUserId(User);
            if (clientId == null) return Unauthorized("Could not find logged-in patient.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.ClientId))
                return BadRequest("ClientId is required for Personnel/Admin.");
            clientId = dto.ClientId;
        }

        if (dto.StartTime >= dto.EndTime)
            return BadRequest("End time must be after start time.");

        var appointment = new Appointment
        {
            AvailabilityId = dto.AvailabilityId,
            ClientId = clientId!,
            TaskDescription = dto.TaskDescription,
            Status = dto.Status,

    

            // Hvis entiteten bruker TimeSpan, bruk:
             StartTime = dto.StartTime.ToTimeSpan(),
             EndTime   = dto.EndTime.ToTimeSpan(),
        };

        await _appointmentRepository.CreateAsync(appointment);

        var resultDto = new AppointmentDto
        {
            Id = appointment.Id,
            AvailabilityId = appointment.AvailabilityId,
            ClientId = appointment.ClientId,
            TaskDescription = appointment.TaskDescription,
            Status = appointment.Status,
            Date = slot.Date,
            ClientName = appointment.Client?.FullName,
            PersonnelId = slot.PersonnelId,
            PersonnelName = slot.Personnel?.FullName,

            // Samme type-kommentar som over:
        
            StartTime = TimeOnly.FromTimeSpan(appointment.StartTime),
            EndTime   = TimeOnly.FromTimeSpan(appointment.EndTime),
            // eller FromTimeSpan(...)
        };

        return CreatedAtAction(nameof(Get), new { id = appointment.Id }, resultDto);
    }

    // PUT: api/appointments/update/5
    [HttpPut("update/{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] AppointmentCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var appt = await _appointmentRepository.GetByIdAsync(id);
        if (appt == null) return NotFound();

        if (dto.StartTime >= dto.EndTime)
            return BadRequest("End time must be after start time.");

        var slot = await _availabilityRepository.GetByIdAsync(dto.AvailabilityId);
        if (slot == null) return BadRequest("Selected availability does not exist.");
        if (slot.Appointment != null && slot.Id != appt.AvailabilityId)
            return BadRequest("This time slot is already booked.");

        if (User.IsInRole("Patient"))
        {
            var userId = _userManager.GetUserId(User);
            if (appt.ClientId != userId) return Forbid();

            appt.Status = "Booked"; // pasient kan ikke endre til hva som helst
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(dto.ClientId))
                appt.ClientId = dto.ClientId!;
            appt.Status = dto.Status;
        }

        appt.AvailabilityId = dto.AvailabilityId;
        appt.TaskDescription = dto.TaskDescription;

    
        // Hvis entiteten bruker TimeSpan:
        appt.StartTime = dto.StartTime.ToTimeSpan();
        appt.EndTime   = dto.EndTime.ToTimeSpan();

        await _appointmentRepository.UpdateAsync(appt);
        return NoContent();
    }

    // DELETE: api/appointments/delete/5
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var appt = await _appointmentRepository.GetByIdAsync(id);
        if (appt == null) return NotFound();

        if (User.IsInRole("Patient"))
        {
            var userId = _userManager.GetUserId(User);
            if (appt.ClientId != userId) return Forbid();
        }

        await _appointmentRepository.DeleteAsync(id);
        return NoContent();
    }
}
