using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Services;
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

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewBag.ProductList = GetProductSelectList();
            return View(new Sale());
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sale model)
        {
            ViewBag.ProductList = GetProductSelectList();

            ModelState.Remove(nameof(Sale.TotalAmount));
            ModelState.Remove(nameof(Sale.Product));

            if (model.ProductId <= 0)
            {
                ModelState.AddModelError(nameof(Sale.ProductId), "Product is required");
            }

            if (model.QuantitySold <= 0)
            {
                ModelState.AddModelError(nameof(Sale.QuantitySold), "Quantity must be at least 1");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = _context.Products.Find(model.ProductId);
            if (product == null)
            {
                ModelState.AddModelError(nameof(Sale.ProductId), "Product not found");
                return View(model);
            }

            if (model.QuantitySold > product.Quantity)
            {
                ModelState.AddModelError(nameof(Sale.QuantitySold),
                    $"Insufficient stock. Available: {product.Quantity}, Requested: {model.QuantitySold}");
                return View(model);
            }

            model.TotalAmount = product.Price * model.QuantitySold;
            product.Quantity -= model.QuantitySold;

            _context.Sales.Add(model);
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Sales/DownloadInvoice/{id}
        public IActionResult DownloadInvoice(int id)
        {
            var sale = _context.Sales
                .Include(s => s.Product)
                .FirstOrDefault(s => s.Id == id);

            if (sale == null)
                return NotFound();

            var pdfBytes = InvoicePdfGenerator.GenerateSaleInvoice(sale);
            return File(pdfBytes, "application/pdf", $"Sale-Invoice-{sale.Id:D5}.pdf");
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
    }
}
