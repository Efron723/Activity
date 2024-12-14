using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace Activity.Controllers
{
    public class HelloWorldController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View("Details");
        }

        public IActionResult Welcome(string name, string nums)
        {
            ViewData["Name"] = "Hello" + name;
            ViewData["Nums"] = int.TryParse(nums, out int ValidNums) ? ValidNums : 0;
            return View();
        }
    }
}
