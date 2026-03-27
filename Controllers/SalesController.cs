using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventoryManagement.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var salesHistory = _context.Sales
                .Include(s => s.Product)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(salesHistory);
        }
    }
}
