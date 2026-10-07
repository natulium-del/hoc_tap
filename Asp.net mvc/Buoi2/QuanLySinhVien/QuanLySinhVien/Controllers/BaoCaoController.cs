using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Services;

namespace QuanLySinhVien.Controllers;

[Route("bao-cao")] // tiền tố cho cả controller
public class BaoCaoController : Controller
{
    private readonly SinhVienService _service;

    public BaoCaoController(SinhVienService service) => _service = service;

    [HttpGet("")] // GET /bao-cao
    public IActionResult Index()
        => Content($"Tổng số sinh viên: {_service.GetAll().Count}");

    [HttpGet("theo-lop/{lop}")] // GET /bao-cao/theo-lop/CNTT-K18
    public IActionResult TheoLop(string lop)
    {
        var ds = _service.GetAll()
            .Where(x => x.Lop == lop).ToList();
        return Json(ds);
    }

    [HttpGet("diem-cao/{nguong:double}")] // GET /bao-cao/diem-cao/8
    public IActionResult DiemCao(double nguong)
    {
        var ds = _service.GetAll()
            .Where(x => x.DiemTrungBinh >= nguong).ToList();
        return Json(ds);
    }
}