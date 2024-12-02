using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LinkUpAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Post")]
        public int PostId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relation : Un commentaire appartient à un post
        public Post Post { get; set; }

        // Relation : Un commentaire appartient à un utilisateur
        public User User { get; set; }
    }
}
