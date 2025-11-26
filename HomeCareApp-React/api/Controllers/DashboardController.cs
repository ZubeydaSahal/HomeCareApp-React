// Controllers/DashboardController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCareApp.Service.User;
using HomeCareApp.Service.Appointments;
using HomeCareApp.Service.Availabilitys;
using HomeCareApp.ViewModels;
using HomeCareApp.Models;

[Authorize] // you have to be logged in
public class DashboardController : Controller
{
    private readonly IUserService _userService;
    private readonly IAppointmentService _appointmentService;
    private readonly IAvailabilityService _availabilityService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IUserService userService,
        IAppointmentService appointmentService,
        IAvailabilityService availabilityService,
        ILogger<DashboardController> logger)
    {
        _userService = userService;
        _appointmentService = appointmentService;
        _availabilityService = availabilityService;
        _logger = logger;
    }

    // ------------------------------------------------------
    // PERSONNEL DASHBOARD
    // ------------------------------------------------------
    [Authorize(Roles = "Personnel,Admin")]
    public async Task<IActionResult> Personnel()
    {
        try
        {
            _logger.LogInformation("Dashboard.Personnel accessed by {User}", User.Identity?.Name);

            var currentUser = await _userService.GetCurrentUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Personnel dashboard access failed: no logged-in user");
                return RedirectToAction("Login", "User");
            }

            var viewModel = await BuildPersonnelViewModelAsync(currentUser.Id);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Personnel dashboard for {User}", User.Identity?.Name);
            TempData["Error"] = "Could not load dashboard.";
            return RedirectToAction("Error", "Home");
        }
    }

    // ------------------------------------------------------
    // PATIENT DASHBOARD
    // ------------------------------------------------------
    [Authorize(Roles = "Patient,Admin")]
    public async Task<IActionResult> Patient()
    {
        try
        {
            _logger.LogInformation("Dashboard.Patient accessed by {User}", User.Identity?.Name);

            var currentUser = await _userService.GetCurrentUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("Patient dashboard access failed: no logged-in user");
                return RedirectToAction("Login", "User");
            }

            var viewModel = await BuildPatientViewModelAsync(currentUser.Id);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Patient dashboard for {User}", User.Identity?.Name);
            TempData["Error"] = "Could not load dashboard.";
            return RedirectToAction("Error", "Home");
        }
    }

    // ------------------------------------------------------
    // BUILD PERSONNEL VIEWMODEL
    // ------------------------------------------------------
    private async Task<PersonnelViewModel> BuildPersonnelViewModelAsync(string personnelId)
    {
        try
        {
            _logger.LogInformation("Building PersonnelViewModel for PersonnelId {Id}", personnelId);

            var personnel = await _userService.GetCurrentUserAsync(User);
            var appointments = await _appointmentService.GetByPersonnelIdAsync(personnelId);
            var upcomingAppointments = await _appointmentService.GetUpcomingByPersonnelIdAsync(personnelId);
            var recentAppointments = await _appointmentService.GetRecentByPersonnelIdAsync(personnelId);
            var upcomingAvailability = await _availabilityService.GetUpcomingByPersonnelIdAsync(personnelId);

            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekEnd = weekStart.AddDays(7);

            var appointmentsThisWeek = appointments
                .Where(a => a.Availability.Date >= weekStart && a.Availability.Date < weekEnd)
                .ToList();

            _logger.LogInformation("Personnel {Id} has {Count} appointments this week",
                personnelId, appointmentsThisWeek.Count);

            return new PersonnelViewModel
            {
                PersonnelId = personnelId,
                PersonnelName = personnel?.FullName ?? "Unknown",
                TotalPatients = appointments.Select(a => a.ClientId).Distinct().Count(),
                AppointmentsThisWeek = appointmentsThisWeek.Count,
                PendingAppointments = appointments.Count(a => a.Status == "Booked"),
                CancelledAppointments = appointments.Count(a => a.Status == "Cancelled"),
                UpcomingAppointments = upcomingAppointments.Take(5).Select(MapToAppointmentSummary).ToList(),
                RecentAppointments = recentAppointments.Select(MapToAppointmentSummary).ToList(),
                UpcomingAvailability = upcomingAvailability.Select(MapToAvailabilitySummary).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building PersonnelViewModel for {Id}", personnelId);
            throw; // rethrow so the caller handles it
        }
    }

    // ------------------------------------------------------
    // BUILD PATIENT VIEWMODEL
    // ------------------------------------------------------
    private async Task<PatientViewModel> BuildPatientViewModelAsync(string patientId)
    {
        try
        {
            _logger.LogInformation("Building PatientViewModel for PatientId {Id}", patientId);

            var patient = await _userService.GetCurrentUserAsync(User);
            var appointments = await _appointmentService.GetByClientIdAsync(patientId);
            var personnel = await _userService.GetPersonnelAsync();

            var today = DateTime.Today;
            var upcomingAppointments = appointments
                .Where(a => a.Availability.Date >= today && a.Status != "Cancelled")
                .ToList();

            var appointmentHistory = appointments
                .Where(a => a.Availability.Date < today || a.Status == "Completed")
                .ToList();

            var availableCaregivers = new List<CaregiverSummary>();

            foreach (var p in personnel)
            {
                var availability = await _availabilityService.GetUpcomingByPersonnelIdAsync(p.Id);
                var nextAvailable = availability.FirstOrDefault();

                if (nextAvailable != null)
                {
                    availableCaregivers.Add(new CaregiverSummary
                    {
                        PersonnelId = p.Id,
                        PersonnelName = p.FullName,
                        Email = p.Email ?? "",
                        AvailableSlots = availability.Count,
                        NextAvailableDate = nextAvailable.Date
                    });
                }
            }

            _logger.LogInformation("{Count} caregivers available for patient {Id}",
                availableCaregivers.Count, patientId);

            return new PatientViewModel
            {
                PatientId = patientId,
                PatientName = patient?.FullName ?? "Unknown",
                UpcomingAppointments = upcomingAppointments.Select(MapToAppointmentSummary).ToList(),
                AppointmentHistory = appointmentHistory.Take(10).Select(MapToAppointmentSummary).ToList(),
                AvailableCaregivers = availableCaregivers.Take(5).ToList(),
                TotalAppointments = appointments.Count,
                CompletedAppointments = appointments.Count(a => a.Status == "Completed")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building PatientViewModel for {Id}", patientId);
            throw; // rethrow to controller action
        }
    }

    // ------------------------------------------------------
    // MAP APPOINTMENT SUMMARY
    // ------------------------------------------------------
    private AppointmentSummary MapToAppointmentSummary(Appointment appointment)
    {
        try
        {
            return new AppointmentSummary
            {
                Id = appointment.Id,
                ClientName = appointment.Client?.FullName ?? "Unknown",
                PersonnelName = appointment.Availability?.Personnel?.FullName ?? "Unknown",
                TaskDescription = appointment.TaskDescription,
                Date = appointment.Availability?.Date ?? DateTime.MinValue,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping AppointmentSummary for appointment {Id}", appointment.Id);
            throw;
        }
    }

    // ------------------------------------------------------
    // MAP AVAILABILITY SUMMARY
    // ------------------------------------------------------
    private AvailabilitySummary MapToAvailabilitySummary(Availability availability)
    {
        try
        {
            return new AvailabilitySummary
            {
                Id = availability.Id,
                Date = availability.Date,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                Notes = availability.Notes,
                IsBooked = availability.Appointment != null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping AvailabilitySummary for availability {Id}", availability.Id);
            throw;
        }
    }

    // ------------------------------------------------------
    // Personalised welcome message
    // ------------------------------------------------------
    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        var userName = User.Identity?.Name ?? "User";
        ViewBag.UserName = userName;
        base.OnActionExecuting(context);
    }
}
