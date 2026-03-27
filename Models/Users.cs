using System;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }

    public string Role { get; set; } = "user";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}