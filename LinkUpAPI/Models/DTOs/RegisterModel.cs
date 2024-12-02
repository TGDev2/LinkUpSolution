using System.ComponentModel.DataAnnotations;

namespace LinkUpAPI.Models.DTOs
{
    public class RegisterModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public bool IsProfilePublic { get; set; } = true;
    }
}
