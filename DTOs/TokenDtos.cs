namespace OnlineStoreBackend.DTOs;

public sealed class RefreshTokenRequest
{
    public required string RefreshToken { get; set; }
}

public sealed class AuthTokenResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime AccessTokenExpiresAt { get; init; }
}
