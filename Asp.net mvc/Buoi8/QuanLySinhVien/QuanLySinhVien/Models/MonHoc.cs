using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class MonHoc
    {
        public  int Id { get; set; }

        [Required, MaxLength(100)]
        public string TenMonHoc { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Số tín chỉ phải nằm trong khoảng từ 1 đến 10")]
        public int SoTinChi { get; set; }

        public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
      

}

