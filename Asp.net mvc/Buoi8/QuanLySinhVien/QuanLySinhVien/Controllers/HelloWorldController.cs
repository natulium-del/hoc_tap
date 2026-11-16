using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
namespace QuanLySinhVien.Controllers
{
    public class HelloWorldController : Controller
    {
        public string Index()
        {
            return "this is my default action.....";
        }

        public string Welcome(string Name,int ID=1)
        {
            return HtmlEncoder.Default.Encode($"Hello {Name}, ID is {ID}");
          
        }
        private string BiMat()
        {
            return "this is a private method...";
        }
    }
}
