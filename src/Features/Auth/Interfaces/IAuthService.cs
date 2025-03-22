using src.Features.Auth.Dtos;
using src.features.user.entities;

namespace src.Features.Auth.Interfaces;

public interface IAuthService
{
    public string GenerateToken(User user);
    public Task<string> RegisterUser(RegisterUserDto dto);

    public Task<string> LoginUser(LoginUserDto dto);
}
