using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Services;
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

        // GET: Purchases/Create
        public IActionResult Create()
        {
            ViewBag.ProductList = GetProductSelectList();
            ViewBag.SupplierList = GetSupplierSelectList();
            ViewBag.UnitPrice = 0m;
            return View(new Purchase());
        }

        // GET: Purchases/GetProductPrice
        [HttpGet]
        public IActionResult GetProductPrice(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            return Json(new { price = product.Price, stock = product.Quantity, name = product.Name });
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Purchase model, decimal unitPrice)
        {
            ViewBag.ProductList = GetProductSelectList();
            ViewBag.SupplierList = GetSupplierSelectList();
            ViewBag.UnitPrice = unitPrice;

            ModelState.Remove(nameof(Purchase.TotalCost));
            ModelState.Remove(nameof(Purchase.Product));
            ModelState.Remove(nameof(Purchase.Supplier));

            if (model.ProductId <= 0)
            {
                ModelState.AddModelError(nameof(Purchase.ProductId), "Product is required");
            }

            if (model.SupplierId <= 0)
            {
                ModelState.AddModelError(nameof(Purchase.SupplierId), "Supplier is required");
            }

            if (model.Quantity <= 0)
            {
                ModelState.AddModelError(nameof(Purchase.Quantity), "Quantity must be at least 1");
            }

            if (unitPrice <= 0)
            {
                ModelState.AddModelError("unitPrice", "Unit price must be greater than 0");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = _context.Products.Find(model.ProductId);
            if (product == null)
            {
                ModelState.AddModelError(nameof(Purchase.ProductId), "Product not found");
                return View(model);
            }

            var supplier = _context.Suppliers.Find(model.SupplierId);
            if (supplier == null)
            {
                ModelState.AddModelError(nameof(Purchase.SupplierId), "Supplier not found");
                return View(model);
            }

            // Update product stock based on purchased quantity
            product.Quantity += model.Quantity;

            // If purchase price changed, update product price as requested
            if (product.Price != unitPrice)
            {
                product.Price = unitPrice;
            }

            model.TotalCost = unitPrice * model.Quantity;

            _context.Purchases.Add(model);
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Purchases/DownloadInvoice/{id}
        public IActionResult DownloadInvoice(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefault(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            var pdfBytes = InvoicePdfGenerator.GeneratePurchaseInvoice(purchase);
            return File(pdfBytes, "application/pdf", $"Purchase-Invoice-{purchase.Id:D5}.pdf");
        }

        private List<SelectListItem> GetProductSelectList()
        {
            return _context.Products
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} (Stock: {p.Quantity}, Price: ₹{p.Price:N2})"
                })
                .ToList();
        }

        private List<SelectListItem> GetSupplierSelectList()
        {
            return _context.Suppliers
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();
        }
    }
}
