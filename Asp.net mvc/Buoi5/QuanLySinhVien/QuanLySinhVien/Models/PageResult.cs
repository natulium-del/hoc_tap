namespace QuanLySinhVien.Models;

public class PageResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TongSo { get; set; }              // tổng số bản ghi thỏa điều kiện
    public int Trang { get; set; }               // trang hiện tại (bắt đầu từ 1)
    public int KichThuoc { get; set; }           // số bản ghi mỗi trang
    public int TongSoTrang => (int)Math.Ceiling(TongSo / (double)KichThuoc);

}



