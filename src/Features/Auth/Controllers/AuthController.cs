using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using src.Features.Auth.Dtos;
using src.Features.Auth.Interfaces;
using src.Features.Auth.Response;

namespace src.Features.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterUserDto> _registerUserDtoValidator;

    private readonly IValidator<LoginUserDto> _loginUserDtoValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterUserDto> registerUserDtoValidator,
        IValidator<LoginUserDto> loginUserDtoValidator
    )
    {
        _authService = authService;
        _registerUserDtoValidator = registerUserDtoValidator;
        _loginUserDtoValidator = loginUserDtoValidator;
    }

    [HttpPost("register")]
    public async Task<string> RegisterUser([FromBody] RegisterUserDto dto)
    {
        var validationResult = _registerUserDtoValidator.Validate(dto);

        if (!validationResult.IsValid)
        {
            throw new Exception("Invalid registration request.");
        }

        return await _authService.RegisterUser(dto);
    }

    [HttpPost("login")]
    public async Task<TokenResponse> LoginUser([FromBody] LoginUserDto dto)
    {
        var validationResult = _loginUserDtoValidator.Validate(dto);

        if (!validationResult.IsValid)
        {
            throw new Exception("Invalid login request.");
        }

        return await _authService.LoginUser(dto);
    }

    [HttpPost("refresh")]
    public Task<TokenRequest> RefreshToken([FromBody] TokenRequest token)
    {
        return _authService.LoginWithRefreshToken(token);
    }
}
