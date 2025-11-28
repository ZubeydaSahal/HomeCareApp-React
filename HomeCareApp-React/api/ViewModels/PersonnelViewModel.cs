using HomeCareApp.Models;

namespace HomeCareApp.ViewModels
{
    /// <summary>
    /// ViewModel for Personnel Dashboard containing all necessary data for display
    /// </summary>
    public class PersonnelViewModel
    {
        public string PersonnelId { get; set; } = string.Empty;
        public string PersonnelName { get; set; } = string.Empty;
        
        // Quick Stats Overview
        public int TotalPatients { get; set; }
        public int AppointmentsThisWeek { get; set; }
        public int PendingAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        
        // Upcoming Appointments
        public List<AppointmentSummary> UpcomingAppointments { get; set; } = new();
        
        // Recent Activity
        public List<AppointmentSummary> RecentAppointments { get; set; } = new();
        
        // Availability Summary
        public List<AvailabilitySummary> UpcomingAvailability { get; set; } = new();
    }

    /// <summary>
    /// Summary information for appointments displayed on dashboard
    /// </summary>
    public class AppointmentSummary
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string PersonnelName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string FormattedDateTime => $"{Date:MMM dd, yyyy} at {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }

    /// <summary>
    /// Summary information for availability displayed on dashboard
    /// </summary>
    public class AvailabilitySummary
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
        public bool IsBooked { get; set; }
        public string FormattedDateTime => $"{Date:MMM dd, yyyy} at {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }
}
