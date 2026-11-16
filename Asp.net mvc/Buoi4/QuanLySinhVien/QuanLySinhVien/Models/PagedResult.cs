namespace QuanLySinhVien.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TongSo { get; set; }
    public int Trang { get; set; }
    public int KichThuoc { get; set; }
    public int TongSoTrang => (int)Math.Ceiling(TongSo / (double)KichThuoc);
}