using System;

namespace HomeCareApp.DTOs
{
    public class AvailabilityDto
    {
        public int Id { get; set; }

        public string PersonnelId { get; set; } = string.Empty;

        // Valgfritt: navn på den ansatte, hentet fra User
        public string? PersonnelName { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }

        // Valgfritt: kun ID til avtale
        public int? AppointmentId { get; set; }

        // Praktisk flagg til frontend
        public bool IsBooked => AppointmentId.HasValue;
    }
}
