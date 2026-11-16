using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace ProjectManager.Models
{
    public class Project 
    {
       public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        public DateOnly StartDate { get; set; } //Sql sever:Date

        [Precision(18,2)]
        public decimal Budget { get; set; } //Sql sever:Decimal(18,2)

        //Phía Nhiều : khóa ngoại + reference navigation -> khách hàng của dự án
        public int CustomerId { get; set; } //Khóa ngoại
        public Customer? Customer { get; set; } //Reference navigation
        public List<Tag> Tags { get; set; } = [];
        public List<Resource> Resources { get; set; } = new(); // skip navigation
        public List<Assignment> Assignments { get; set; } = new(); // tới entity nối (đọc Role, JoinedOn)

    }
}
