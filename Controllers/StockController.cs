using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventoryManagement.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var stockHistory = _context.StockTransactions
                .Include(st => st.Product)
                .OrderByDescending(st => st.CreatedAt)
                .ToList();
             
            return View(stockHistory);
        }
    }
}
