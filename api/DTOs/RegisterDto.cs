using System.ComponentModel.DataAnnotations;

namespace HomeCareApp.DTOs
{
    public class RegisterDto
    {
        [Required]
        [RegularExpression(@"[a-zA-ZæøåÆØÅ \s\-]{2,50}$", ErrorMessage = "The name must contain only letters, spaces, periods, or hyphens.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Patient"; // default
    }
}
