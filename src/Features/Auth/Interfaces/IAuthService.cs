using src.Features.Auth.Dtos;
using src.Features.Auth.Dtos.Response;

namespace src.Features.Auth.Interfaces;

public interface IAuthService
{
    public Task<RegisterUserDto> RegisterUser(RegisterUserDto dto);

    public Task<TokenResponse> LoginUser(LoginUserDto dto);

    public Task<TokenRequest> LoginWithRefreshToken(TokenRequest request);
}
