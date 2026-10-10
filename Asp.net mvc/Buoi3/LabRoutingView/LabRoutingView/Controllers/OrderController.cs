using Microsoft.AspNetCore.Mvc;

namespace LabRoutingView.Controllers
{
    [Route("orders")]
    public class OrderController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Content("This is the Orders Index page.");
        }

        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            return Content($"This is the details page for Order ID: {id}");
        }

        [HttpGet("{id:int}/items")]
        public IActionResult Items(int id)
        {
            return Content($"This is the items page for Order ID: {id}");
        }

        [HttpPost("create")]
        public IActionResult Create()
        {
            return Content("This is the create page for Orders.");
        }

    }
}
