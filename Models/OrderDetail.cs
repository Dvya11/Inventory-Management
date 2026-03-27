using System;
using System.ComponentModel.DataAnnotations;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Product Product { get; set; }
}
