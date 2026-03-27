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
            
            var allProducts = _context.Products.ToList();
            
            ViewBag.AllProducts = allProducts;
             
            return View(stockHistory);
        }

        // GET: Stock/Create
        public IActionResult Create()
        {
            var products = _context.Products.ToList();
            ViewBag.Products = products;
            ViewBag.TransactionTypes = new[] { "IN", "OUT" };
            return View();
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StockTransaction model)
        {
            // Always reload products for the dropdown
            ViewBag.Products = _context.Products.ToList();
            ViewBag.TransactionTypes = new[] { "IN", "OUT" };
            
            // ✅ EXPLICIT VALIDATION - Ensure ProductId is not 0
            if (model.ProductId <= 0)
            {
                ModelState.AddModelError("ProductId", "Product is required");
            }
            
            // ✅ Check Type is selected
            if (string.IsNullOrWhiteSpace(model.Type))
            {
                ModelState.AddModelError("Type", "Transaction type is required");
            }
            
            // ✅ Check Quantity is valid
            if (model.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be at least 1");
            }
            
            // If any validation failed, return form
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Find the product
            var product = _context.Products.Find(model.ProductId);
            
            if (product == null)
            {
                ModelState.AddModelError("ProductId", "Product not found");
                return View(model);
            }

            // Handle stock OUT transaction
            if (model.Type == "OUT")
            {
                if (product.Quantity < model.Quantity)
                {
                    ModelState.AddModelError("Quantity", 
                        $"Insufficient stock. Available: {product.Quantity}, Requested: {model.Quantity}");
                    return View(model);
                }
                product.Quantity -= model.Quantity;
            }
            // Handle stock IN transaction
            else if (model.Type == "IN")
            {
                product.Quantity += model.Quantity;
            }

            // Save the transaction
            _context.StockTransactions.Add(model);
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "Product");
        }
    }
}
