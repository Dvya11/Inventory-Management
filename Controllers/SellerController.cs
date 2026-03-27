using Microsoft.AspNetCore.Mvc;
using System;

public class SellerController : Controller
{
    private readonly ApplicationDbContext _context;

    public SellerController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Register
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(Seller seller)
    {
        if (ModelState.IsValid)
        {
            _context.Sellers.Add(seller);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        return View(seller);
    }

    // Login
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var seller = _context.Sellers
            .FirstOrDefault(x => x.Email == email && x.Password == password);

        if (seller != null)
        {
            HttpContext.Session.SetInt32("SellerId", seller.SellerId);
            return RedirectToAction("Dashboard");
        }

        ViewBag.Error = "Invalid Email or Password";
        return View();
    }

    // Dashboard
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("SellerId") == null)
            return RedirectToAction("Login");

        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Index()
    {
        int sellerId = (int)HttpContext.Session.GetInt32("SellerId");

        var products = _context.Products
            .Where(p => p.SellerId == sellerId)
            .ToList();

        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        product.SellerId = (int)HttpContext.Session.GetInt32("SellerId");

        _context.Products.Add(product);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}