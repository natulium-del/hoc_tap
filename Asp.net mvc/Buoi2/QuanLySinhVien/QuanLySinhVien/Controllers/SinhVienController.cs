// Import namespace chứa các lớp MVC (Controller, IActionResult, View, NotFound...)
using Microsoft.AspNetCore.Mvc;

// Import namespace chứa Model SinhVien của dự án
using QuanLySinhVien.Models;

// Khai báo namespace gốc của dự án (dùng file-scoped namespace, C# 10+)
namespace QuanLySinhVien.Controllers;

using QuanLySinhVien.Services;
// Lớp SinhVienController kế thừa từ Controller (lớp cơ sở cung cấp View(), NotFound(), ViewData...)
public class SinhVienController : Controller
{
    // Danh sách sinh viên tĩnh (static) — dữ liệu mẫu, dùng chung cho mọi request
    // readonly: không thể gán lại _danhSach, nhưng vẫn có thể thêm/xóa phần tử
    private static readonly List<SinhVien> _danhSach = new()
    {
        // Khởi tạo 3 đối tượng SinhVien với dữ liệu mẫu
        new SinhVien { Id = 1, HoTen = "Trần Văn Bình", Lop = "CNTT-K18", DiemTrungBinh = 7.8 },
        new SinhVien { Id = 2, HoTen = "Lê Thị Cúc",    Lop = "CNTT-K18", DiemTrungBinh = 8.5 },
        new SinhVien { Id = 3, HoTen = "Phạm Văn Dũng", Lop = "CNTT-K19", DiemTrungBinh = 6.9 }
    };

    // Action mặc định — truy cập qua /SinhVien hoặc /SinhVien/Index
    //public IActionResult Index()
    //{
    // Truyền toàn bộ danh sách sang View (View sẽ dùng @model List<SinhVien>)
    //    return View(_danhSach);
    //}

    // Action xem chi tiết — truy cập qua /SinhVien/Detail/1 hoặc /SinhVien/Detail?id=1
    // Tham số id được bind tự động từ route hoặc query string
    //public IActionResult Detail(int id)
    //{
    // Tìm sinh viên có Id khớp trong danh sách
    // FirstOrDefault trả về null nếu không tìm thấy
    //var sv = _danhSach.FirstOrDefault(x => x.Id == id);

    // Nếu không tìm thấy → trả về HTTP 404 (NotFound)
    //if (sv == null) return NotFound();

    // Tìm thấy → truyền đối tượng SinhVien sang View chi tiết
    //    return View(sv);
    //}
    private readonly SinhVienService _service;

    public SinhVienController(SinhVienService service)
    {
        _service = service;
    }

    // GET: /SinhVien?tuKhoa=cnt
    public IActionResult Index(string? tuKhoa)
    {
        ViewData["Title"] = "Danh sách sinh viên";
        ViewData["TuKhoa"] = tuKhoa;
        return View(_service.TimKiem(tuKhoa));
    }

    // GET: /SinhVien/Detail/2

    public IActionResult Detail(int id)
    {
        var sv = _service.Find(id);
        if (sv is null) return NotFound();

        ViewData["Title"] = "Chi tiết sinh viên";        // dữ liệu phụ
        ViewBag.NamHoc = "2025 - 2026";                  // dynamic
        ViewBag.TongSoSV = _service.GetAll().Count;      // ⚠️ thiếu ()
        TempData["GhiChu"] = "Dữ liệu chỉ mang tính minh họa";

        return View(sv); // model chính
    }
    public IActionResult Search(string? keyword)
    {
        // Nếu không có từ khóa → trả về danh sách rỗng
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return View(new List<SinhVien>());
        }

        // Tìm theo tên hoặc lớp, không phân biệt hoa thường
        var ketQua = _danhSach
            .Where(sv =>
                sv.HoTen.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                sv.Lop.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Truyền từ khóa sang View để hiển thị lại trong ô nhập
        ViewData["Keyword"] = keyword;

        return View(ketQua);
    }
    // 1. ContentResult - trả về chuỗi văn bản thuần
    public IActionResult XinChao()
        => Content("Xin chào lớp CNTT!", "text/plain; charset=utf-8");

    // 2. JsonResult - trả về dữ liệu JSON
    public IActionResult DanhSachJson()
        => Json(_service.GetAll());

    // 3. FileResult - trả về file để tải xuống
    public IActionResult TaiDanhSach()
    {
        var noiDung = "Id,HoTen,Lop,DiemTB\n" + string.Join("\n",
            _service.GetAll().Select(x =>
                $"{x.Id},{x.HoTen},{x.Lop},{x.DiemTrungBinh}"));
        var bytes = System.Text.Encoding.UTF8.GetBytes(noiDung);
        return File(bytes, "text/csv", "DanhSachSinhVien.csv");
    }
    // 4. StatusCodeResult - trả về mã lỗi máy chủ
    public IActionResult LoiMayChu()
        => StatusCode(500, "Có lỗi xảy ra phía máy chủ");

    // 5. EmptyResult - không trả về nội dung
    public IActionResult KhongCoGi() => new EmptyResult();
    // GET: /SinhVien/Them - hiển thị biểu mẫu
    [HttpGet]
    public IActionResult Them()
    {
        return View();
    }

    // POST: /SinhVien/Them - nhận dữ liệu và lưu
    [HttpPost]
    public IActionResult Them(SinhVien sv)
    {
        _service.Them(sv);
        TempData["ThongBao"] = $"Đã thêm sinh viên {sv.HoTen} thành công!";
        return RedirectToAction(nameof(Index)); // POST-Redirect-GET
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Xoa(int id)
    {
        var sv = _service.Find(id);
        if (sv is null)
        {
            TempData["Loi"] = "Không tìm thấy sinh viên cần xóa!";
            return RedirectToAction(nameof(Index));
        }

        _service.Xoa(id);
        TempData["ThongBao"] = $"Đã xóa sinh viên {sv.HoTen} thành công!";
        return RedirectToAction(nameof(Index)); // POST-Redirect-GET
    }
}