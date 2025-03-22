using Microsoft.AspNetCore.Mvc;
using src.Features.Auth.Dtos;
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
    public async Task<string> RegisterUser([FromBody] RegisterUserDto dto)
    {
        return await _authService.RegisterUser(dto);
    }

    [HttpPost("login")]
    public async Task<string> LoginUser([FromBody] LoginUserDto dto)
    {
        return await _authService.LoginUser(dto);
    }
}
