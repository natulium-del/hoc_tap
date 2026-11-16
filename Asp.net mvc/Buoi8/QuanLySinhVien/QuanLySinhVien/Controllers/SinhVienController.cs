using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.DTOs;
using QuanLySinhVien.Models;
using QuanLySinhVien.Services;
using QuanLySinhVien.ViewModels;

namespace QuanLySinhVien.Controllers;

[ApiController] // Bật validation tự động
[Route("api/[controller]")]
public class SinhVienController(ISinhVienService service, AppDbContext _db, IWebHostEnvironment env) : Controller
{
    // ==========================================
    // 1. DANH SÁCH & CHI TIẾT
    // ==========================================
    public async Task<IActionResult> Index(string? tuKhoa, string? sapXep, int trang = 1)
    {
        ViewBag.TuKhoa = tuKhoa;
        ViewBag.SapXep = sapXep;
        return View(await service.TimKiemAsync(tuKhoa, sapXep, trang));
    }

    public async Task<IActionResult> Detail(int id)
    {
        var sv = await service.FindWithKhoaAsync(id);
        return sv is null ? NotFound() : View(sv);
    }

    // ==========================================
    // 2. THÊM SINH VIÊN (Đã tích hợp Bước 2: Nghiệp vụ)
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Them()
    {
        var vm = new SinhVienCreateVm();
        await NapDanhSachKhoa(vm.KhoaId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(SinhVienCreateVm vm)
    {
        // BƯỚC 1: Luật khai báo bằng Data Annotations + IValidatableObject
        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoa(vm.KhoaId);
            return View(vm);
        }

        // BƯỚC 2: Luật nghiệp vụ (Cần truy vấn Database)
        // 2.1 Kiểm tra trùng Mã sinh viên
        if (await _db.SinhViens.AnyAsync(x => x.MaSV == vm.MaSV))
        {
            ModelState.AddModelError(nameof(vm.MaSV), $"Mã sinh viên {vm.MaSV} đã tồn tại trong hệ thống");
        }

        // 2.2 Kiểm tra sĩ số lớp (tối đa 40)
        int siSo = await _db.SinhViens.CountAsync(x => x.Lop == vm.Lop);
        if (siSo >= 40)
        {
            // Lỗi chung của form (hiển thị ở ValidationSummary)
            ModelState.AddModelError(string.Empty, $"Lớp {vm.Lop} đã đủ sĩ số 40 sinh viên, không thể thêm.");
        }

        // Nếu có bất kỳ lỗi nghiệp vụ nào, trả lại form
        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoa(vm.KhoaId);
            return View(vm);
        }

        // BƯỚC 3: Thực hiện nghiệp vụ (Xử lý File & Ánh xạ)
        string? tenFileLuu = null;
        if (vm.AnhDaiDien is { Length: > 0 } file)
        {
            var duoi = Path.GetExtension(file.FileName).ToLowerInvariant();
            tenFileLuu = $"{Guid.NewGuid():N}{duoi}";

            var thuMuc = Path.Combine(env.WebRootPath, "uploads");
            Directory.CreateDirectory(thuMuc);

            var duongDan = Path.Combine(thuMuc, tenFileLuu);
            await using var stream = System.IO.File.Create(duongDan);
            await file.CopyToAsync(stream);
        }

        // Ánh xạ thủ công sang Entity (Chống Over-posting)
        var sv = new SinhVien
        {
            MaSV = vm.MaSV,
            HoTen = vm.HoTen,
            Lop = vm.Lop,
            NgaySinh = vm.NgaySinh,
            Email = vm.Email,
            DiemTrungBinh = vm.DiemTrungBinh,
            KhoaId = vm.KhoaId,
            AnhDaiDien = tenFileLuu
        };

        await service.ThemAsync(sv); // Hoặc _db.SinhViens.Add(sv); await _db.SaveChangesAsync();

        // BƯỚC 4: POST-Redirect-GET
        TempData["ThongBao"] = $"Đã thêm sinh viên {sv.HoTen} thành công.";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // 3. SỬA
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Sua(int id)
    {
        var sv = await service.FindAsync(id);
        if (sv is null) return NotFound();

        await NapDanhSachKhoa(sv.KhoaId);
        return View(sv);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(SinhVien sv)
    {
        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoa(sv.KhoaId);
            return View(sv);
        }

        if (!await service.CapNhatAsync(sv)) return NotFound();
        TempData["ThongBao"] = "Cập nhật thành công!";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // 4. XÓA MỀM & KHÔI PHỤC
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id)
    {
        TempData["ThongBao"] = await service.XoaAsync(id)
            ? "Đã xóa sinh viên."
            : "Không tìm thấy sinh viên cần xóa.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ThungRac()
        => View(await service.GetDaXoaAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhoiPhuc(int id)
    {
        TempData["ThongBao"] = await service.KhoiPhucAsync(id)
            ? "Đã khôi phục sinh viên."
            : "Không tìm thấy bản ghi.";
        return RedirectToAction(nameof(ThungRac));
    }

    // ==========================================
    // 5. CÁC TÍNH NĂNG KHÁC
    // ==========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CongDiem(string lop, double mucCong)
    {
        int soDong = await service.CongDiemChoLopAsync(lop, mucCong);
        TempData["ThongBao"] = $"Đã cập nhật điểm cho {soDong} sinh viên.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ThiNghiem()
    {
        Console.WriteLine("===== BAT DAU: PHIEN BAN N+1 =====");
        await service.BaoCaoN1Async();

        Console.WriteLine("===== BAT DAU: PHIEN BAN INCLUDE =====");
        await service.BaoCaoIncludeAsync();

        Console.WriteLine("===== BAT DAU: PHIEN BAN PROJECTION =====");
        var kq = await service.BaoCaoProjectionAsync();

        return Content(string.Join("\n", kq), "text/plain; charset=utf-8");
    }

    // ==========================================
    // 6. NHẬP ĐIỂM HÀNG LOẠT (Dùng _db)
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> NhapDiem(string lop)
    {
        var ds = await _db.SinhViens.AsNoTracking()
            .Where(x => x.Lop == lop)
            .OrderBy(x => x.HoTen)
            .Select(x => new DongDiemVm
            {
                SinhVienId = x.Id,
                HoTen = x.HoTen,
                Diem = x.DiemTrungBinh
            })
            .ToListAsync();

        return View(new NhapDiemVm { Lop = lop, DanhSach = ds });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NhapDiem(NhapDiemVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        foreach (var dong in vm.DanhSach.Where(d => d.Diem.HasValue))
        {
            var sv = await _db.SinhViens.FindAsync(dong.SinhVienId);
            if (sv is not null) sv.DiemTrungBinh = dong.Diem!.Value;
        }

        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Đã cập nhật điểm cho {vm.DanhSach.Count} sinh viên.";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // 7. THÍ NGHIỆM LỖ HỔNG (Over-posting)
    // ==========================================
    [HttpGet]
    public IActionResult ThemKhongAnToan()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemKhongAnToan(SinhVien sv)
    {
        _db.SinhViens.Add(sv);
        await _db.SaveChangesAsync();

        return Content($"Đã thêm {sv.HoTen} | LaQuanTri = {sv.LaQuanTri}", "text/plain; charset=utf-8");
    }

    // ==========================================
    // HÀM HỖ TRỢ
    // ==========================================
    private async Task NapDanhSachKhoa(int? daChon = null)
    {
        var khoas = await service.GetKhoasAsync();
        ViewBag.KhoaList = new SelectList(khoas, "Id", "TenKhoa", daChon);
    }
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> KiemTraMaSV(string maSV, int id = 0)
    {
        if (string.IsNullOrWhiteSpace(maSV))
            return Json(true);

        // Kiểm tra xem mã này đã tồn tại trong DB chưa (loại trừ bản ghi đang sửa)
        bool daTonTai = await _db.SinhViens
            .AnyAsync(x => x.MaSV == maSV && x.Id != id);

        // true = hợp lệ (không trùng)
        // false = không hợp lệ -> jQuery hiển thị ErrorMessage
        return Json(!daTonTai);
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Tao(TaoSinhVienRequest req)
    {
        // KHÔNG cần if (!ModelState.IsValid)
        // Framework đã chặn từ trước và trả 400 kèm ValidationProblemDetails

        // Kiểm tra nghiệp vụ: Mã sinh viên đã tồn tại chưa?
        if (await _db.SinhViens.AnyAsync(x => x.MaSV == req.MaSV))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(req.MaSV)] = ["Mã sinh viên đã tồn tại"]
            }));
        }

        // Ánh xạ DTO sang Entity
        var sv = new SinhVien
        {
            MaSV = req.MaSV,
            HoTen = req.HoTen,
            DiemTrungBinh = req.DiemTrungBinh,
            KhoaId = req.KhoaId
        };

        _db.SinhViens.Add(sv);
        await _db.SaveChangesAsync();

        // Trả về 201 Created kèm đường dẫn tới API chi tiết (nếu có)
        return CreatedAtAction(nameof(Tao), new { id = sv.Id }, null);
    }
}