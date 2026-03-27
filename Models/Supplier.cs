using System;
using System.ComponentModel.DataAnnotations;

public class Supplier
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string ContactPerson { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
    
    public string Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
