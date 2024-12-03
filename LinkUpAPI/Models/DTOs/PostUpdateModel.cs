using System.ComponentModel.DataAnnotations;

namespace LinkUpAPI.Models.DTOs
{
    public class PostUpdateModel
    {
        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public string MediaUrl { get; set; }
    }
}