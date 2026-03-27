using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventoryManagement.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public PurchasesController(ApplicationDbContext context) 
        { 
            _context = context; 
        }

        public IActionResult Index()
        {
            var purchases = _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return View(purchases);
        }
    }
}
