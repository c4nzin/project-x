using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using src.contexts;
using src.Features.Auth.Dtos;
using src.Features.Auth.Interfaces;
using src.features.user.entities;
using src.Utils;

namespace src.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly string _jwtKey;

    public readonly UserManager<User> _userManager;

    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IConfiguration configuration,
        UserManager<User> userManager,
        ILogger<AuthService> logger
    )
    {
        _jwtKey = configuration["JWT:Key"];
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<string> RegisterUser(RegisterUserDto dto)
    {
        var isUserExist = await _userManager.FindByEmailAsync(dto.Email);

        if (isUserExist != null)
        {
            throw new Exception("User is already registered.");
        }

        var user = new User { Email = dto.Email, UserName = dto.Name };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            foreach (var errors in result.Errors)
            {
                _logger.LogError($"Registration error : {errors.Description}");
            }
        }

        return "User registered successfully.";
    }

    public async Task<string> LoginUser(LoginUserDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!result)
        {
            throw new Exception("Invalid password.");
        }

        return GenerateToken(user);
    }

    //move to jwt service
    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_jwtKey);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(user),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims = new ClaimsIdentity();

        claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));

        return claims;
    }
}
