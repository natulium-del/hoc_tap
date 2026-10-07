using QuanLySinhVien.Models;

namespace QuanLySinhVien.Services;

/// <summary>
/// Service quản lý danh sách sinh viên (in-memory).
/// Cung cấp các thao tác: lấy danh sách, tìm kiếm, thêm, xóa.
/// </summary>
public class SinhVienService
{
    // Danh sách sinh viên dùng chung cho mọi instance (static).
    // readonly: chỉ gán 1 lần, không cho phép gán lại tham chiếu.
    // Lưu ý: readonly KHÔNG ngăn việc Add/Remove phần tử bên trong list.
    private static readonly List<SinhVien> _ds = new()
    {
        new SinhVien { Id = 1, HoTen = "Trần Văn Bình", Lop = "CNTT-K18", DiemTrungBinh = 7.8 },
        new SinhVien { Id = 2, HoTen = "Lê Thị Cúc",    Lop = "CNTT-K18", DiemTrungBinh = 8.5 },
        new SinhVien { Id = 3, HoTen = "Phạm Văn Dũng", Lop = "CNTT-K19", DiemTrungBinh = 6.9 }
    };

    /// <summary>
    /// Lấy toàn bộ danh sách sinh viên.
    /// </summary>
    /// <returns>Danh sách sinh viên hiện có.</returns>
    public List<SinhVien> GetAll() => _ds;

    /// <summary>
    /// Tìm sinh viên theo Id.
    /// </summary>
    /// <param name="id">Mã sinh viên cần tìm.</param>
    /// <returns>Sinh viên nếu tìm thấy, ngược lại null.</returns>
    public SinhVien? Find(int id) => _ds.FirstOrDefault(x => x.Id == id);

    /// <summary>
    /// Tìm kiếm sinh viên theo từ khóa (tên hoặc lớp).
    /// Không phân biệt hoa/thường.
    /// </summary>
    /// <param name="tuKhoa">Từ khóa tìm kiếm. Nếu rỗng → trả về toàn bộ danh sách.</param>
    /// <returns>Danh sách sinh viên khớp từ khóa.</returns>
    public List<SinhVien> TimKiem(string? tuKhoa)
    {
        // Nếu từ khóa rỗng hoặc chỉ có khoảng trắng → trả về tất cả
        if (string.IsNullOrWhiteSpace(tuKhoa)) return _ds;

        // Lọc theo HoTen hoặc Lop, không phân biệt hoa/thường
        return _ds.Where(x =>
                x.HoTen.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase) ||
                x.Lop.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Thêm sinh viên mới vào danh sách.
    /// Id sẽ được tự động sinh (max Id hiện tại + 1).
    /// </summary>
    /// <param name="sv">Đối tượng sinh viên cần thêm.</param>
    public void Them(SinhVien sv)
    {
        // Nếu danh sách rỗng → Id = 1, ngược lại = max(Id) + 1
        sv.Id = _ds.Count == 0 ? 1 : _ds.Max(x => x.Id) + 1;
        _ds.Add(sv);
    }

    /// <summary>
    /// Xóa sinh viên theo Id.
    /// </summary>
    /// <param name="id">Mã sinh viên cần xóa.</param>
    /// <returns>true nếu xóa thành công, false nếu không tìm thấy.</returns>
    public bool Xoa(int id)
    {
        var sv = Find(id);
        return sv is not null && _ds.Remove(sv);
    }
}