using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
