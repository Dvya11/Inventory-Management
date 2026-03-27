using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Please login first.";
                return RedirectToAction("Login", "Account");
            }

            return View("Dashboard");
        }
    }
}
