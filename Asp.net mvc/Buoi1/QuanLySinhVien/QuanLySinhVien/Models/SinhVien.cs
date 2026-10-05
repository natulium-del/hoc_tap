namespace QuanLySinhVien.Models
{
    public class SinhVien
    {
        public int Id { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Lop { get; set; } = string.Empty;
        public double DiemTrungBinh { get; set; }
    }
}
