using Microsoft.AspNetCore.Mvc;
using System;

public class SellerOrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public SellerOrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        int sellerId = (int)HttpContext.Session.GetInt32("SellerId");

        var orders = _context.OrderDetails
            .Where(o => o.Product.SellerId == sellerId)
            .ToList();

        return View(orders);
    }
}