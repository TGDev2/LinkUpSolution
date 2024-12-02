using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LinkUpAPI.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        public string MediaUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Relation : Un post appartient à un utilisateur
        public User User { get; set; }

        // Relation : Un post peut avoir plusieurs commentaires
        public ICollection<Comment> Comments { get; set; }

        public ICollection<Media> Media { get; set; }
    }
}
