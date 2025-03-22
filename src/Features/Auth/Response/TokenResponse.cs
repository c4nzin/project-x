namespace src.Features.Auth.Response;

public sealed record TokenResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
