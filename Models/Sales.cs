using System;
using System.ComponentModel.DataAnnotations;

public class Sale
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Product is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
    public int QuantitySold { get; set; }

    [Required(ErrorMessage = "Total amount is required")]
    [Range(0.01, 100000, ErrorMessage = "Amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Product Product { get; set; }
}