using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Cần nếu sau này dùng [ForeignKey]

namespace ProjectManager.Models;

public class ResourceProfile
{
    [Key] // Báo cho EF biết đây là Khóa chính (vì tên không phải là "Id")
    public int ResourceId { get; set; }

    [Required, StringLength(15)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Bio { get; set; }

    // Navigation property trỏ về bảng cha (Resource)
    // Dùng null! để tắt cảnh báo nullable của C# (vì EF Core sẽ tự gán giá trị khi query)
    public Resource Resource { get; set; } = null!;
}