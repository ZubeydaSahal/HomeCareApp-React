using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HomeCareApp.ViewModels.Appointment
{
    public class AppointmentCreateViewModel
    {
        public string? ClientId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Please choose an available day/slot.")]
        public int AvailabilityId { get; set; }

        [Required(ErrorMessage = "Task description is required.")]
        public string TaskDescription { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required]
        [RegularExpression(@"^(Booked|Completed|Cancelled)$")]
        public string Status { get; set; } = "Booked";

        // For dropdowns
        public SelectList? AvailabilityOptions { get; set; }
        public SelectList? ClientOptions { get; set; }
        public bool IsPersonnel { get; set; }
    }
}
