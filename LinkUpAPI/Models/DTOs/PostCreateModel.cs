using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUpAPI.Models.DTOs
{
    public class PostCreateModel
    {
        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public IFormFile MediaFile { get; set; }
    }
}
