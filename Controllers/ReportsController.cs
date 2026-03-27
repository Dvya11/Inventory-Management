using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
