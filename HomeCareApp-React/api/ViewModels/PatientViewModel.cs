using HomeCareApp.Models;

namespace HomeCareApp.ViewModels
{
    /// <summary>
    /// ViewModel for Patient Dashboard containing all necessary data for display
    /// </summary>
    public class PatientViewModel
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        
        // Upcoming Appointments
        public List<AppointmentSummary> UpcomingAppointments { get; set; } = new();
        
        // Appointment History
        public List<AppointmentSummary> AppointmentHistory { get; set; } = new();
        
        // Available Caregivers
        public List<CaregiverSummary> AvailableCaregivers { get; set; } = new();
        
        // Quick Stats
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int UpcomingCount => UpcomingAppointments.Count;
    }

    /// <summary>
    /// Summary information for caregivers displayed on patient dashboard
    /// </summary>
    public class CaregiverSummary
    {
        public string PersonnelId { get; set; } = string.Empty;
        public string PersonnelName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
        public DateTime NextAvailableDate { get; set; }
        public string FormattedNextAvailable => NextAvailableDate.ToString("MMM dd, yyyy");
    }
}
