namespace HomeCareApp.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        public int AvailabilityId { get; set; }

        public string? ClientId { get; set; }
        public string? ClientName { get; set; }

        public string? PersonnelId { get; set; }
        public string? PersonnelName { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string? TaskDescription { get; set; }
        public string? Status { get; set; }
    }
}
