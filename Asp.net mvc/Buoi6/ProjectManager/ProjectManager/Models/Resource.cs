using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models
{
    public abstract class Resource
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = "";

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; } = "";

        // Navigation properties
        public ResourceProfile? Profile { get; set; }
        public List<Project> Projects { get; set; } = new();
        public List<Assignment> Assignments { get; set; } = new();
    }

    
    public class Developer : Resource
    {
        [StringLength(30)]
        public string MainLanguage { get; set; } = "";
        public int YearsOfExperience { get; set; }
    }

    public class Tester : Resource
    {
        [StringLength(50)]
        public string ToolName { get; set; } = "";
    }
}