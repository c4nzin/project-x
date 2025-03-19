using Microsoft.AspNetCore.Mvc;
using src.Features.Auth.Dtos;
using src.Features.Auth.Interfaces;
using src.Features.Auth.Services;
using src.features.user.entities;

namespace src.Features.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService; // Inject IAuthService

    public AuthController(IAuthService authService) // Constructor takes IAuthService
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<User> RegisterUser([FromBody] RegisterUserDto dto)
    {
        return await _authService.RegisterUser(dto);
    }
}
