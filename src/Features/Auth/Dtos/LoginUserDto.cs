using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace src.Features.Auth.Dtos;

public class LoginUserDto
{
    public required string Email { get; set; }

    public required string Password { get; set; }
}

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}
