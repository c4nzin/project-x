using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using src.contexts;
using src.Features.Auth.Dtos;
using src.Features.Auth.Interfaces;
using src.Features.Auth.Response;
using src.Features.Auth.Token;
using src.features.user.entities;
using DbContext = src.contexts.DbContext;

namespace src.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly string _jwtKey;

    public readonly UserManager<User> _userManager;

    public readonly DbContext _dbContext;

    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IConfiguration configuration,
        UserManager<User> userManager,
        ILogger<AuthService> logger,
        DbContext dbContext
    )
    {
        _jwtKey = configuration["JWT:Key"];
        _userManager = userManager;
        _logger = logger;
        _dbContext = dbContext;
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

    public async Task<TokenResponse> LoginUser(LoginUserDto dto)
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

        var accessToken = GenerateToken(user);

        var refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(3),
            Id = Guid.NewGuid(),
        };

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync();

        return new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
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

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public async Task<TokenRequest> LoginWithRefreshToken(TokenRequest request)
    {
        var refreshToken = await _dbContext
            .RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (refreshToken == null)
        {
            throw new Exception("Invalid refresh token.");
        }

        if (refreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            throw new Exception("Refresh token expired.");
        }

        string accessToken = GenerateToken(refreshToken.User);

        refreshToken.Token = GenerateRefreshToken();

        refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(3);

        await _dbContext.SaveChangesAsync();

        return new TokenRequest { RefreshToken = refreshToken.Token };
    }
}
