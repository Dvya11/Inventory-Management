using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, 100000, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Low stock threshold is required")]
    [Range(1, 10000, ErrorMessage = "Threshold must be at least 1")]
    public int LowStockThreshold { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<StockTransaction> StockTransactions { get; set; }
}