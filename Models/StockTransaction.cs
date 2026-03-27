using System;
using System.ComponentModel.DataAnnotations;

public class StockTransaction
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Product is required")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Transaction type is required")]
    [RegularExpression(@"^(IN|OUT)$", ErrorMessage = "Type must be either 'IN' or 'OUT'")]
    public string Type { get; set; } // IN / OUT

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Product Product { get; set; }
}