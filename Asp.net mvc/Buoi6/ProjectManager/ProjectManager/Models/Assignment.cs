using System.ComponentModel.DataAnnotations;
namespace ProjectManager.Models;
// Entity nối: mỗi dòng = một nhân sự tham gia một dự án
public class Assignment
{
    public int ProjectId { get; set; } // PK (phần 1) + FK
    public int ResourceId { get; set; } // PK (phần 2) + FK
    [Required, StringLength(50)]
    public string Role { get; set; } = ""; // payload
    public DateOnly JoinedOn { get; set; } // payload
    public Project Project { get; set; } = null!;
    public Resource Resource { get; set; } = null!;
}