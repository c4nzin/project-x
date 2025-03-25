using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using src.Features.Auth.Interfaces;
using src.Features.Auth.Services;
using src.features.user.entities;

namespace src.Utils;

public static class ServiceExtensions
{
    public static void ConfigureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
    }

    public static void ConfigureDbContext(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<src.contexts.DbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<src.contexts.DbContext>()
            .AddDefaultTokenProviders();
    }
}
