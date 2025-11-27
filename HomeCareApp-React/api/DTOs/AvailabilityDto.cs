using System.ComponentModel.DataAnnotations;

namespace HomeCareApp.DTOs
{
    public class AvailabilityDto
    {
        public string PersonnelId { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }
    }
}