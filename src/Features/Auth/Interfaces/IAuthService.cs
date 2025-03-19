using src.Features.Auth.Dtos;
using src.features.user.entities;

namespace src.Features.Auth.Interfaces;

public interface IAuthService
{
    public string GenerateToken(User user);
    public Task<User> RegisterUser(RegisterUserDto dto);
}
