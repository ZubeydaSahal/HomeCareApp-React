using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCareApp.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }  

        [Required]
        [Display(Name = "Client")]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Availability")]
        public int AvailabilityId { get; set; }

        [Required(ErrorMessage = "Task description is required.")]
        [StringLength(200, ErrorMessage = "Task description cannot be longer than 200 characters.")]
        public string TaskDescription { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required]
        [RegularExpression(@"^(Booked|Completed|Cancelled)$", ErrorMessage = "Invalid status value.")]
        public string Status { get; set; } = "Booked";

        public string? ClientName { get; set; }
        public string? PersonnelName { get; set; }
    }
}
