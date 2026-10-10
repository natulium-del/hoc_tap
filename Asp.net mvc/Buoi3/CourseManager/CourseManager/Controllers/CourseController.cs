using Microsoft.AspNetCore.Mvc;
using CourseManager.Models;

namespace CourseManager.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index(int page = 1)
        {
            const int pageSize = 4;
            var all = CourseRepository.Courses;
            var totalPages = (int)Math.Ceiling(all.Count / (double)pageSize);

            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageTitle = "Danh sách khóa học";
            ViewBag.Total = all.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }
        public IActionResult ByCategory(string category)
        {
            var items = CourseRepository.Courses
                .Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
            ViewBag.PageTitle = $"Khóa học: {category}";
            ViewBag.Total = items.Count;

            return View("Index",items);
        }

        public IActionResult Detail(int id)
        {
            var course = CourseRepository.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return View("NotFound");
            }
            ViewBag.PageTitle = $"Chi tiết khóa học: {course.Title}";
            return View(course);
        }
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string name, string email, int courseId)
        {
            TempData["Success"] = $"Đăng ký thành công khóa học #{courseId} cho {name}!";
            return RedirectToAction("Index");
        }

        public IActionResult ByCode(string code)
        {
            var course = CourseRepository.Courses
                .FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (course == null) return View("NotFound");
            return View("Detail", course);
        }
    }
}
