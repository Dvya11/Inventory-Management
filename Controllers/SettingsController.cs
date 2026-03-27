using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
