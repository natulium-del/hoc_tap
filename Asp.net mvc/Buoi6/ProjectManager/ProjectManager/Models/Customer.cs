using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
namespace ProjectManager.Models
{
    public class Customer 
    {
       public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [StringLength(100),EmailAddress]
        public string? Email { get; set; }

        //Phía một Collection nativagtion -> danh sách sự án của khách hàng
        public List<Project> Projects { get; set; } = [];
    }
}
