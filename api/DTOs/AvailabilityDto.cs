using System;

namespace HomeCareApp.DTOs
{
    public class AvailabilityDto
    {
        public int Id { get; set; }

        public string PersonnelId { get; set; } = string.Empty;


        public string? PersonnelName { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }

        public int? AppointmentId { get; set; }

    }
}
