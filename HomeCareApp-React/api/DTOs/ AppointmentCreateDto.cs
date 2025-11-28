// Fil: DTOs/AppointmentCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace HomeCareApp.DTOs
{
    public class AppointmentCreateDto
    {
        [Required]
        public int AvailabilityId { get; set; }   // Hvilket ledig time-slot

        // Pasient: dette settes fra token (ignoreres i request)
        // Personnel/Admin: kan sende inn ClientId eksplisitt
        public string? ClientId { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        public string? TaskDescription { get; set; }

        // F.eks. "Booked", "Cancelled", "Completed"
        public string Status { get; set; } = "Booked";
    }
}
