using System.Security.Claims;
using src.features.user.entities;

namespace src.Features.Auth.Interfaces;

public interface ITokenService
{
    public ClaimsIdentity GenerateClaims(User user);

    public string GenerateRefreshToken();

    public string GenerateToken(User user);
}
