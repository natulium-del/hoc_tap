using Microsoft.AspNetCore.Mvc;

namespace CourseManager.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "backend", "frontend", "mobile"
        public decimal Price { get; set; }
        public int Duration { get; set; } // số giờ học
        public bool IsPublished { get; set; }
        public string Code { get; set; } = string.Empty; // KH-0001
    }
}
