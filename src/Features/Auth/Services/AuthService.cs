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

namespace src.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _jwtKey; //burada jwt keyi saklamak icin tutuyorum

        public readonly UserDbContext _context;

        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration configuration,
            UserDbContext context,
            ILogger<AuthService> logger
        )
        {
            _jwtKey = configuration["JWT:Key"];
            _context = context;
            _logger = logger;
        }

        public async Task<User> RegisterUser(RegisterUserDto dto)
        {
            var isUserExist = await _context.Users.AnyAsync(u =>
                u.Email == dto.Email || u.Name == dto.Name
            );

            if (isUserExist)
            {
                throw new Exception("User is already registered.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(null, dto.Password);

            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                Password = hashedPassword,
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user to database.");
                throw new Exception("Error saving user to database.");
            }

            return user;
        }

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
}
