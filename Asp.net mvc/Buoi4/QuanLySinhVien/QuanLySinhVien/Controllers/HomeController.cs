using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Models;
using System.Diagnostics;

namespace QuanLySinhVien.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public IActionResult Index()
        {
            ViewData["HoTen"] = "Nguyễn Văn A";
            ViewData["MSSV"] = "SV2024001";
            ViewData["Lop"] = "CNTT-K18";
            ViewData["ThoiGian"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            ViewData["TenLop"] = _config["ThongTinLop:TenLop"];
            ViewData["GiangVien"] = _config["ThongTinLop:TenGiangVien"];
            ViewData["SoSinhVien"] = _config.GetValue<int>("ThongTinLop:SoSinhVien");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? id)
        {
            // Lấy mã lỗi từ route — mặc định là 500 nếu không có
            int statusCode = id ?? 500;
            ViewData["StatusCode"] = statusCode;
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            return View();
        }
        public IActionResult GayLoi()
        {
            throw new InvalidOperationException(
                "Đây là lỗi cố ý để kiểm thử middleware xử lý ngoại lệ.");
        }

      
    }
}
