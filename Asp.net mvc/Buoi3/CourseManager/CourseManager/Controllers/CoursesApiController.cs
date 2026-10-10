using Microsoft.AspNetCore.Mvc;
using CourseManager.Models;
namespace CourseManager.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll() => Ok(CourseRepository.Courses);

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var course = CourseRepository.Courses.FirstOrDefault(c => c.Id == id);
            return course==null ? NotFound() : Ok(course);
        }
    }
}
