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
[Route("api/appointments")]
[Authorize(Roles = "Personnel,Patient,Admin")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly UserManager<AppUser> _userManager;

    public AppointmentController(
        IAppointmentRepository appointmentRepository,
        IAvailabilityRepository availabilityRepository,
        UserManager<AppUser> userManager)
    {
        _appointmentRepository = appointmentRepository;
        _availabilityRepository = availabilityRepository;
        _userManager = userManager;
    }

    private string? GetCurrentUserId()
    {
        var nameIdClaims = User.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .ToList();

        if (!nameIdClaims.Any())
            return null;

        var userId = nameIdClaims.Last().Value;

        Console.WriteLine(
            $"[AppointmentApiController] NameId claims: {string.Join(" | ", nameIdClaims.Select(c => c.Value))}. Using userId={userId}");

        return userId;
    }

    // ----------------------------------------------------
    // GET: api/appointments/list
    // - Patient: sees own appointments
    // - Personnel: all appointments where they are assigned
    // - Admin: all appointments
    // ----------------------------------------------------
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> List()
    {
        var all = await _appointmentRepository.GetAllAsync() ?? new List<Appointment>();

        var userId = GetCurrentUserId();
        var isPatient = User.IsInRole("Patient");
        var isPersonnel = User.IsInRole("Personnel");
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && !string.IsNullOrEmpty(userId))
        {
            if (isPatient)
            {
                all = all.Where(a => a.ClientId == userId).ToList();
            }
            else if (isPersonnel)
            {
                all = all
                    .Where(a => a.Availability != null &&
                                a.Availability.PersonnelId == userId)
                    .ToList();
            }
        }

        var dtos = all.Select(a => new AppointmentDto
        {
            Id = a.Id,
            AvailabilityId = a.AvailabilityId,
            ClientId = a.ClientId,
            ClientName = a.Client?.FullName,
            PersonnelId = a.Availability?.PersonnelId,
            PersonnelName = a.Availability?.Personnel?.FullName,
            Date = a.Availability?.Date ?? default,

            // The entity has TimeSpan, DTO has TimeOnly
            StartTime = TimeOnly.FromTimeSpan(a.StartTime),
            EndTime = TimeOnly.FromTimeSpan(a.EndTime),

            TaskDescription = a.TaskDescription,
            Status = a.Status
        });

        return Ok(dtos);
    }

    // ----------------------------------------------------
    // GET: api/appointments/
    // ----------------------------------------------------
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> Get(int id)
    {
        var appt = await _appointmentRepository.GetByIdAsync(id);
        if (appt == null) return NotFound();

        var userId = GetCurrentUserId();
        var isPatient = User.IsInRole("Patient");
        var isPersonnel = User.IsInRole("Personnel");
        var isAdmin = User.IsInRole("Admin");

        // Admin can always
        if (!isAdmin && !string.IsNullOrEmpty(userId))
        {
            if (isPatient && appt.ClientId != userId)
            {
                return Forbid();
            }

            if (isPersonnel)
            {
                var personnelId = appt.Availability?.PersonnelId;
                if (personnelId != userId)
                {
                    return Forbid();
                }
            }
        }

        var dto = new AppointmentDto
        {
            Id = appt.Id,
            AvailabilityId = appt.AvailabilityId,
            ClientId = appt.ClientId,
            ClientName = appt.Client?.FullName,
            PersonnelId = appt.Availability?.PersonnelId,
            PersonnelName = appt.Availability?.Personnel?.FullName,
            Date = appt.Availability?.Date ?? default,
            StartTime = TimeOnly.FromTimeSpan(appt.StartTime),
            EndTime = TimeOnly.FromTimeSpan(appt.EndTime),
            TaskDescription = appt.TaskDescription,
            Status = appt.Status
        };

        return Ok(dto);
    }


    // ----------------------------------------------------
    // POST: api/appointments/create
    // - Patient: can only book for themselves
    // - Personnel/Admin: must provide ClientId in DTO
    // ----------------------------------------------------
    [HttpPost("create")]
    public async Task<ActionResult> Create([FromBody] AppointmentCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var slot = await _availabilityRepository.GetByIdAsync(dto.AvailabilityId);
        if (slot == null) return BadRequest("Selected availability does not exist.");
        if (slot.Appointment != null) return BadRequest("This time slot is already booked.");

        // parse tider fra string → TimeSpan
        if (!TimeSpan.TryParse(dto.StartTime, out var startTs) ||
            !TimeSpan.TryParse(dto.EndTime, out var endTs))
        {
            return BadRequest("Invalid time format. Use HH:mm.");
        }

        if (startTs >= endTs)
            return BadRequest("End time must be after start time.");

        string? clientId;

        if (User.IsInRole("Patient"))
        {
            clientId = GetCurrentUserId();
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Could not find logged-in patient.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.ClientId))
                return BadRequest("ClientId is required for Personnel/Admin.");
            clientId = dto.ClientId;
        }

        var appointment = new Appointment
        {
            AvailabilityId = dto.AvailabilityId,
            ClientId = clientId!,
            TaskDescription = dto.TaskDescription,
            Status = dto.Status,
            StartTime = startTs,
            EndTime = endTs
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
            StartTime = TimeOnly.FromTimeSpan(appointment.StartTime),
            EndTime = TimeOnly.FromTimeSpan(appointment.EndTime),
        };

        return CreatedAtAction(nameof(Get), new { id = appointment.Id }, resultDto);
    }

    // ----------------------------------------------------
    // PUT: api/appointments/update/5
    // - Patient: can only update their own, and we lock Status to "Booked"
    // - Personnel/Admin: can update everything
    // ----------------------------------------------------
    [HttpPut("update/{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] AppointmentCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var appt = await _appointmentRepository.GetByIdAsync(id);
        if (appt == null) return NotFound();

        if (!TimeSpan.TryParse(dto.StartTime, out var startTs) ||
            !TimeSpan.TryParse(dto.EndTime, out var endTs))
        {
            return BadRequest("Invalid time format. Use HH:mm.");
        }

        if (startTs >= endTs)
            return BadRequest("End time must be after start time.");

        var slot = await _availabilityRepository.GetByIdAsync(dto.AvailabilityId);
        if (slot == null) return BadRequest("Selected availability does not exist.");
        if (slot.Appointment != null && slot.Id != appt.AvailabilityId)
            return BadRequest("This time slot is already booked.");

        if (User.IsInRole("Patient"))
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            if (appt.ClientId != userId) return Forbid();

            // Patient cannot set Completed/Cancelled themselves
            appt.Status = "Booked";
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(dto.ClientId))
                appt.ClientId = dto.ClientId!;
            appt.Status = dto.Status;
        }

        appt.AvailabilityId = dto.AvailabilityId;
        appt.TaskDescription = dto.TaskDescription;
        appt.StartTime = startTs;
        appt.EndTime = endTs;

        await _appointmentRepository.UpdateAsync(appt);
        return NoContent();
    }

    // ----------------------------------------------------
    // DELETE: api/appointments/delete/5
    // - Patient: can only delete their own
    // - Personnel/Admin: can delete all
    // ----------------------------------------------------
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var appt = await _appointmentRepository.GetByIdAsync(id);
        if (appt == null) return NotFound();

        if (User.IsInRole("Patient"))
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            if (appt.ClientId != userId) return Forbid();
        }

        await _appointmentRepository.DeleteAsync(id);
        return NoContent();
    }
}
