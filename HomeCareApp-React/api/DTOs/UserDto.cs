using System.ComponentModel.DataAnnotations;

namespace HomeCareApp.DTOs
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for the User model.
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Full Name")] 
        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty; // "Personnel" or "Patient"

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }
}