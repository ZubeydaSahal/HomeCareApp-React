using System.ComponentModel.DataAnnotations;

namespace HomeCareApp.DTOs;
public class AppointmentCreateDto
{
    //DTO is used when a new apointment is created
    public int AvailabilityId { get; set; }
    public string? ClientId { get; set; }
    public string TaskDescription { get; set; } = "";
    public string StartTime { get; set; } = "";
    public string EndTime { get; set; } = "";
    public string Status { get; set; } = "Booked";
}

