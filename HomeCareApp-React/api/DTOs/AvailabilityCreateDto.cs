using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCareApp.DTOs
{
    public class AvailabilityCreateDto
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public string? Notes { get; set; }
    }
}
