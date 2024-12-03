using System;

namespace LinkUpAPI.Models.DTOs
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserViewModel User { get; set; }
    }
}