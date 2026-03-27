using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }
    }
}
