namespace MagicalKitties.Contracts.Requests.Auth;

public class TokenRequest
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}