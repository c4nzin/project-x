namespace src.Features.Auth.Response;

public sealed record TokenRequest
{
    public string RefreshToken { get; init; }
}
