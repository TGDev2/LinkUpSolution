using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace LinkUpAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsProfilePublic { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Post> Posts { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
