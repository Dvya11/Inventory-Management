using System;
using System.ComponentModel.DataAnnotations;

public class Purchase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal TotalCost { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Product Product { get; set; }
    public Supplier Supplier { get; set; }
}
