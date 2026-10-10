using Microsoft.AspNetCore.Mvc;
using LabRoutingView.Models;
namespace LabRoutingView.Controllers
{
    public class ProductController : Controller
    {
        //public IActionResult Index()
        //{
        //    ViewData["Title"] = "Danh sach san pham";
        //    ViewBag.Message = "Chao mung den voi trang san pham";
        //    ViewData["Items"] = new List<string> { "Laptop", "Mouse", "Keyboard" };
        //    return View();
        //}
        private static readonly List<Product> _allProducts = new()
    {
        new Product { Id = 1, Name = "Laptop Dell XPS 13",    Price = 25000000 },
        new Product { Id = 2, Name = "Chuột Logitech MX",     Price = 1500000  },
        new Product { Id = 3, Name = "Bàn phím cơ Keychron",  Price = 2200000  },
        new Product { Id = 4, Name = "Màn hình LG 27\"",      Price = 5500000  },
        new Product { Id = 5, Name = "Tai nghe Sony WH-1000", Price = 7000000  }
    };
        public IActionResult Index()
        {
            return View(_allProducts);
        }
        public IActionResult Save()
        {
            TempData["Result"] = "Luu du lieu thanh cong!";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Details(int id)
        {
            return Content($"This is the details page for Product ID: {id}");
        }

        public IActionResult Category(string name)
        {
            return Content($"This is the category page for Category: {name}");
        }

        public IActionResult Archive(int year, int month)
        {
            return Content($"This is the archive page for Year: {year}, Month: {month}");
        }

        [HttpPost]
        public IActionResult Search(string keyword)
        {
            // 1. Lọc sản phẩm theo từ khóa
            var results = _allProducts
                .Where(p => p.Name.Contains(keyword ?? "", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 2. Gửi từ khóa lại View để hiển thị
            ViewBag.Keyword = keyword;

            // 3. Render lại Index với danh sách đã lọc
            return View("Index", results);
        }
    }
}
