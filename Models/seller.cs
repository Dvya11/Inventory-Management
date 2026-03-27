using System.ComponentModel.DataAnnotations;

public class Seller
{
    [Key]
    public int SellerId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }


}  