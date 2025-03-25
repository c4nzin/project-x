namespace src.Features.Auth.Dtos.Response;

public sealed record TokenRequest
{
    public required string RefreshToken { get; init; }
}
