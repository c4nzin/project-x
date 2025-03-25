using Microsoft.AspNetCore.Mvc;
using src.Features.Auth.Dtos;
using src.Features.Auth.Dtos.Response;
using src.Features.Auth.Interfaces;

namespace src.Features.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<RegisterUserDto> RegisterUser([FromBody] RegisterUserDto dto)
    {
        return await _authService.RegisterUser(dto);
    }

    [HttpPost("login")]
    public async Task<TokenResponse> LoginUser([FromBody] LoginUserDto dto)
    {
        return await _authService.LoginUser(dto);
    }

    [HttpPost("refresh")]
    public async Task<TokenRequest> RefreshToken([FromBody] TokenRequest token)
    {
        return await _authService.LoginWithRefreshToken(token);
    }
}
