using System.ComponentModel.DataAnnotations;

namespace src.Features.Auth.Dtos;

public class LoginUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(
        255, // Increase the maximum length
        MinimumLength = 6,
        ErrorMessage = "Password must be between 6 and 255 characters."
    )]
    public required string Password { get; set; }
}
