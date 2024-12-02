using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LinkUpAPI.Models
{
    public class Media
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Post")]
        public int PostId { get; set; }

        [Required]
        [StringLength(50)]
        public string MediaType { get; set; }

        [Required]
        public string MediaUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relation : Un média est lié à un post
        public Post Post { get; set; }
    }
}
