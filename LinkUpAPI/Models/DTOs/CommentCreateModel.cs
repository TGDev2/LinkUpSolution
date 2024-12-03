using System.ComponentModel.DataAnnotations;

namespace LinkUpAPI.Models.DTOs
{
    public class CommentCreateModel
    {
        [Required]
        [StringLength(250)]
        public string Content { get; set; }
    }
}
