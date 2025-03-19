using System.ComponentModel.DataAnnotations;

namespace src.features.user.entities;

public class User
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(
        15,
        MinimumLength = 3,
        ErrorMessage = "Name must be between 3 and 15 characters."
    )]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(
        255,
        MinimumLength = 6,
        ErrorMessage = "Password must be between 6 and 30 characters."
    )]
    public required string Password { get; set; }
}
