using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using src.Features.Auth.Token;
using src.features.user.entities;

namespace src.contexts;

public class DbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly IConfiguration _configuration;

    public DbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
