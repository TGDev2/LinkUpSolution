using System;

namespace LinkUpAPI.Models.DTOs
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserViewModel User { get; set; }
    }
}
