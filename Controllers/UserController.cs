using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View("Dashboard");
        }

        public IActionResult Dashboard()
        {
            // render the existing Views/User/Index.cshtml
            return View("Index");
        }
    }
}
