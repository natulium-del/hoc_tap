using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class Khoa 
    {
        public int Id { get; set; }
        [Required, MaxLength(10)]
        public string MaKhoa { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string TenKhoa { get; set; } = string.Empty;

        public ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
    }
}
