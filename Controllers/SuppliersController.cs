using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public SuppliersController(ApplicationDbContext context) 
        { 
            _context = context; 
        }

        public IActionResult Index()
        {
            var suppliers = _context.Suppliers.ToList();
            return View(suppliers);
        }

        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Supplier());
            else
            {
                var supplier = _context.Suppliers.Find(id);
                if (supplier == null) return NotFound();
                return View(supplier);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(Supplier supplier)
        {
            ModelState.Remove("ContactPerson");
            ModelState.Remove("Phone");
            ModelState.Remove("Email");

            if (ModelState.IsValid)
            {
                if (supplier.Id == 0)
                    _context.Suppliers.Add(supplier);
                else
                    _context.Suppliers.Update(supplier);

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
