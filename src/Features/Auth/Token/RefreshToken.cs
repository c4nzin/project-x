using src.features.user.entities;

namespace src.Features.Auth.Token;

public class RefreshToken
{
    public Guid Id { get; set; }

    public string Token { get; set; }

    public User User { get; set; }

    public Guid UserId { get; set; }

    public DateTime ExpiresOnUtc { get; set; }
}
