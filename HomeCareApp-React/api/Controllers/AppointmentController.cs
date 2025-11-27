using HomeCareApp.Models;
using HomeCareApp.Service.Appointments;
using HomeCareApp.Service.Availabilities;
using HomeCareApp.ViewModels.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog; 
using AppUser = HomeCareApp.Models.User;
using HomeCareApp.DTOs;

namespace HomeCareApp.Controllers
{
    
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAvailabilityService _availabilityService;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(
            IAppointmentService appointmentService,
            IAvailabilityService availabilityService,
            UserManager<AppUser> userManager)
        {
            _appointmentService = appointmentService;
            _availabilityService = availabilityService;
            _userManager = userManager;
        }

        // ===========================================================
        // INDEX -show all appointments
        // ===========================================================
        [Authorize(Roles = "Personnel,Patient,Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                Log.Information("AppointmentController.Index called by User {User}", User.Identity?.Name);

                if (User.IsInRole("Personnel") || User.IsInRole("Admin"))
                {
                    var all = await _appointmentService.GetAllAsync() ?? new List<Appointment>();
                    Log.Information("Loaded {Count} appointments for Personnel/Admin", all.Count);
                    return View(all);
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    Log.Warning("Challenge returned in Index because userId was null");
                    return Challenge();
                }

                var mine = await _appointmentService.GetByClientIdAsync(userId) ?? new List<Appointment>();
                Log.Information("Loaded {Count} appointments for patient {UserId}", mine.Count, userId);
                return View(mine);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in AppointmentController.Index");
                TempData["Error"] = "An unexpected error occurred.";
                return RedirectToAction("Error", "Home");
            }
        }

        // ===========================================================
        // CREATE (GET)
        // ===========================================================
        [Authorize(Roles = "Personnel,Patient,Admin")]
        public async Task<IActionResult> Create()
        {
            try
            {
                Log.Information("AppointmentController.Create(GET) called by {User}", User.Identity?.Name);

                var availabilities = await _availabilityService.GetAllAsync() ?? new List<Availability>();
                var freeSlots = availabilities.Where(a => a.Appointment == null).ToList();

                var isStaff = User.IsInRole("Personnel") || User.IsInRole("Admin");

                var vm = new AppointmentCreateViewModel
                {
                    IsPersonnel = isStaff,
                    AvailabilityOptions = new SelectList(
                        freeSlots.Select(a => new
                        {
                            a.Id,
                            Display = $"{a.Personnel?.UserName ?? "Ukjent"} - {a.Date:yyyy-MM-dd} {a.StartTime:hh\\:mm}-{a.EndTime:hh\\:mm}"
                        }),
                        "Id", "Display"
                    )
                };

                if (isStaff)
                {
                    var patients = await _userManager.GetUsersInRoleAsync("Patient");
                    vm.ClientOptions = new SelectList(patients, "Id", "UserName");
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading Appointment Create(GET)");
                TempData["Error"] = "An unexpected error occurred.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===========================================================
        // CREATE (POST)
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Personnel,Patient,Admin")]
        public async Task<IActionResult> Create(AppointmentCreateViewModel vm)
        {
            try
            {
                Log.Information("AppointmentController.Create(POST) called with AvailabilityId {AvailabilityId}", vm.AvailabilityId);

                if (User.IsInRole("Patient"))
                {
                    vm.ClientId = _userManager.GetUserId(User);
                    if (string.IsNullOrEmpty(vm.ClientId))
                    {
                        Log.Warning("Patient Create failed: userId null");
                        return Challenge();
                    }
                }
                else if (string.IsNullOrWhiteSpace(vm.ClientId))
                {
                    ModelState.AddModelError(nameof(vm.ClientId), "Please choose a client.");
                }

                if (vm.StartTime >= vm.EndTime)
                    ModelState.AddModelError(nameof(vm.EndTime), "End time must be after start time.");

                var slot = await _availabilityService.GetByIdAsync(vm.AvailabilityId);
                if (slot is null)
                {
                    ModelState.AddModelError(nameof(vm.AvailabilityId), "Selected availability does not exist.");
                }
                else if (slot.Appointment != null)
                {
                    ModelState.AddModelError(nameof(vm.AvailabilityId), "This time slot is already booked.");
                }

                if (!ModelState.IsValid)
                {
                    Log.Warning("Appointment Create validation failed for user {User}", User.Identity?.Name);

                    var availabilities = await _availabilityService.GetAllAsync() ?? new List<Availability>();
                    var freeSlots = availabilities.Where(a => a.Appointment == null).ToList();
                    var isStaff = User.IsInRole("Personnel") || User.IsInRole("Admin");

                    vm.IsPersonnel = isStaff;
                    vm.AvailabilityOptions = new SelectList(
                        freeSlots.Select(a => new
                        {
                            a.Id,
                            Display = $"{a.Personnel?.UserName ?? "Ukjent"} - {a.Date:yyyy-MM-dd} {a.StartTime:hh\\:mm}-{a.EndTime:hh\\:mm}"
                        }),
                        "Id", "Display", vm.AvailabilityId
                    );

                    if (isStaff)
                    {
                        var patients = await _userManager.GetUsersInRoleAsync("Patient");
                        vm.ClientOptions = new SelectList(patients, "Id", "UserName", vm.ClientId);
                    }

                    return View(vm);
                }

                var appointment = new Appointment
                {
                    ClientId = vm.ClientId!,
                    AvailabilityId = vm.AvailabilityId,
                    TaskDescription = vm.TaskDescription,
                    StartTime = vm.StartTime,
                    EndTime = vm.EndTime,
                    Status = vm.Status
                };

                await _appointmentService.CreateAsync(appointment);
                Log.Information("Appointment {AppointmentId} created by {User}", appointment.Id, User.Identity?.Name);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating appointment");
                TempData["Error"] = "Could not create appointment.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===========================================================
        // EDIT (GET)
        // ===========================================================
        [Authorize(Roles = "Personnel,Patient,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Log.Information("Loading appointment {Id} for Edit(GET)", id);

                var appt = await _appointmentService.GetByIdAsync(id);
                if (appt == null)
                {
                    Log.Warning("Edit(GET) NotFound for appointment {Id}", id);
                    return NotFound();
                }

                var isPatient = User.IsInRole("Patient");
                if (isPatient)
                {
                    var userId = _userManager.GetUserId(User);
                    if (appt.ClientId != userId)
                    {
                        Log.Warning("Patient {User} attempted to edit appointment {Id} they do not own", userId, id);
                        return Forbid();
                    }

                    var apptStart = (appt.Availability?.Date ?? DateTime.Today).Add(appt.StartTime);
                    if (apptStart <= DateTime.Now.AddHours(24))
                    {
                        TempData["Error"] = "Rescheduling appointments must be more than 24 hours in advance.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                var isStaff = User.IsInRole("Personnel") || User.IsInRole("Admin");

                var vm = new AppointmentCreateViewModel
                {
                    AvailabilityId = appt.AvailabilityId,
                    TaskDescription = appt.TaskDescription,
                    StartTime = appt.StartTime,
                    EndTime = appt.EndTime,
                    Status = appt.Status,
                    ClientId = appt.ClientId,
                    IsPersonnel = isStaff
                };

                var allAvail = await _availabilityService.GetAllAsync() ?? new List<Availability>();
                var freeSlots = allAvail.Where(a => a.Appointment == null || a.Id == appt.AvailabilityId).ToList();

                vm.AvailabilityOptions = new SelectList(
                    freeSlots.Select(a => new
                    {
                        a.Id,
                        Display = $"{a.Personnel?.UserName ?? "Ukjent"} - {a.Date:yyyy-MM-dd} {a.StartTime:hh\\:mm}-{a.EndTime:hh\\:mm}"
                    }),
                    "Id", "Display", vm.AvailabilityId
                );

                if (vm.IsPersonnel)
                {
                    var patients = await _userManager.GetUsersInRoleAsync("Patient");
                    vm.ClientOptions = new SelectList(patients, "Id", "UserName", vm.ClientId);
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading Edit(GET) for appointment {Id}", id);
                TempData["Error"] = "Could not load edit page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===========================================================
        // EDIT (POST)
        // ===========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Personnel,Patient,Admin")]
        public async Task<IActionResult> Edit(int id, AppointmentCreateViewModel vm)
        {
            try
            {
                Log.Information("Appointment Edit(POST) called for Id {Id}", id);

                var appt = await _appointmentService.GetByIdAsync(id);
                if (appt == null)
                {
                    Log.Warning("Edit(POST) NotFound for appointment {Id}", id);
                    return NotFound();
                }

                var isPatient = User.IsInRole("Patient");
                var isStaff = User.IsInRole("Personnel") || User.IsInRole("Admin");

                if (isPatient)
                {
                    var userId = _userManager.GetUserId(User);
                    if (appt.ClientId != userId)
                    {
                        Log.Warning("Patient {User} attempted to edit appointment {Id} they do not own", userId, id);
                        return Forbid();
                    }

                    var apptStart = (appt.Availability?.Date ?? DateTime.Today).Add(appt.StartTime);
                    if (apptStart <= DateTime.Now.AddHours(24))
                    {
                        TempData["Error"] = "You can only edit appointments at least 24 hours in advance.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                if (vm.StartTime >= vm.EndTime)
                    ModelState.AddModelError(nameof(vm.EndTime), "End time must be after start time.");

                var selected = await _availabilityService.GetByIdAsync(vm.AvailabilityId);
                if (selected is null)
                    ModelState.AddModelError(nameof(vm.AvailabilityId), "Selected availability does not exist.");
                else if (selected.Appointment != null && selected.Id != appt.AvailabilityId)
                    ModelState.AddModelError(nameof(vm.AvailabilityId), "This time slot is already booked.");

                if (!isStaff)
                    vm.ClientId = appt.ClientId;

                if (!ModelState.IsValid)
                {
                    Log.Warning("Appointment Edit validation failed for Id {Id}", id);

                    var allAvail = await _availabilityService.GetAllAsync() ?? new List<Availability>();
                    var freeSlots = allAvail.Where(a => a.Appointment == null || a.Id == appt.AvailabilityId).ToList();

                    vm.IsPersonnel = isStaff;
                    vm.AvailabilityOptions = new SelectList(
                        freeSlots.Select(a => new
                        {
                            a.Id,
                            Display = $"{a.Personnel?.UserName ?? "Unknown"} - {a.Date:yyyy-MM-dd} {a.StartTime:hh\\:mm}-{a.EndTime:hh\\:mm}"
                        }),
                        "Id", "Display", vm.AvailabilityId
                    );

                    if (isStaff)
                    {
                        var patients = await _userManager.GetUsersInRoleAsync("Patient");
                        vm.ClientOptions = new SelectList(patients, "Id", "UserName", vm.ClientId);
                    }

                    return View(vm);
                }

                appt.AvailabilityId = vm.AvailabilityId;
                appt.TaskDescription = vm.TaskDescription;
                appt.StartTime = vm.StartTime;
                appt.EndTime = vm.EndTime;

                if (isStaff)
                {
                    appt.Status = vm.Status;
                    appt.ClientId = vm.ClientId!;
                }
                else
                {
                    appt.Status = "Booked";
                }

                await _appointmentService.UpdateAsync(appt);
                Log.Information("Appointment {Id} updated successfully by {User}", id, User.Identity?.Name);

                TempData["Success"] = "Appointment updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in Appointment Edit(POST) for {Id}", id);
                TempData["Error"] = "Could not update appointment.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===========================================================
        // DELETE
        // ===========================================================
        [Authorize(Roles = "Personnel,Patient,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Log.Information("Delete(GET) called for appointment {Id}", id);

                var appointment = await _appointmentService.GetByIdAsync(id);
                if (appointment == null)
                {
                    Log.Warning("Delete(GET) NotFound for appointment {Id}", id);
                    return NotFound();
                }

                if (User.IsInRole("Patient"))
                {
                    var userId = _userManager.GetUserId(User);
                    if (appointment.ClientId != userId)
                    {
                        Log.Warning("Patient {User} attempted to delete appointment {Id} they do not own", userId, id);
                        return Forbid();
                    }
                }

                return View(appointment);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading Delete(GET) for appointment {Id}", id);
                TempData["Error"] = "Could not load delete page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Personnel,Patient,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Log.Information("DeleteConfirmed(POST) called for appointment {Id}", id);

                var appointment = await _appointmentService.GetByIdAsync(id);
                if (appointment == null)
                {
                    Log.Warning("DeleteConfirmed(POST) NotFound for appointment {Id}", id);
                    return NotFound();
                }

                if (User.IsInRole("Patient"))
                {
                    var userId = _userManager.GetUserId(User);
                    if (appointment.ClientId != userId)
                    {
                        Log.Warning("Patient {User} attempted to delete appointment {Id} they do not own", userId, id);
                        return Forbid();
                    }

                    var slotDate = appointment.Availability?.Date.Add(appointment.Availability?.StartTime ?? TimeSpan.Zero);

                    if (slotDate <= DateTime.Now.AddDays(1))
                    {
                        TempData["Error"] = "Appointment cancellations less than 24 hours before are not allowed.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                await _appointmentService.DeleteAsync(id);
                Log.Information("Appointment {Id} successfully deleted by {User}", id, User.Identity?.Name);

                TempData["Success"] = "Appointment cancelled successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting appointment {Id}", id);
                TempData["Error"] = "Could not delete appointment.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
