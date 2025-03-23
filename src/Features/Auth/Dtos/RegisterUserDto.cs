using FluentValidation;

namespace src.Features.Auth.Dtos;

public class RegisterUserDto
{
    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(2, 100)
            .WithMessage("Name must be between 2 and 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .Length(6, 255)
            .WithMessage("Password must be between 6 and 255 characters");
    }
}
