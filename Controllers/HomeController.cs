using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {        // GET: Login Page (First Page)
        public IActionResult Index()
        {
            return View();
        }
        // POST: Login Action
        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            if (email == "admin@gmail.com" && password == "1234")
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        public IActionResult Dashboard()
        {
            return Content("Welcome to Dashboard!");
        }
    }
    
}