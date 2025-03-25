using src.Features.Auth.Dtos;
using src.Features.Auth.Response;
using src.features.user.entities;

namespace src.Features.Auth.Interfaces;

public interface IAuthService
{
    public string GenerateToken(User user);
    public Task<string> RegisterUser(RegisterUserDto dto);

    public Task<TokenResponse> LoginUser(LoginUserDto dto);

    public string GenerateRefreshToken();

    public Task<TokenRequest> LoginWithRefreshToken(TokenRequest request);
}
