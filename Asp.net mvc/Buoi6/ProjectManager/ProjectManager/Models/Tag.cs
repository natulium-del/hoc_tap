using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models
{
    public class Tag 
    {
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Name { get; set; } = "";
        public List<Project> Projects { get; set; } = []; // phía thứ nhất
    }
}
