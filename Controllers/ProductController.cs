using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace InventoryManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public ProductController(ApplicationDbContext context) 
        { 
            _context = context; 
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Create / Edit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Product());
            else
            {
                var product = _context.Products.Find(id);
                if (product == null) return NotFound();
                return View(product);
            }
        }

        // POST: Create / Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(Product product)
        {
            ModelState.Remove("StockTransactions");
            
            if (ModelState.IsValid)
            {
                if (product.Id == 0)
                    _context.Products.Add(product);
                else
                    _context.Products.Update(product);

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
