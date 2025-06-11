namespace MagicalKitties.Application.Models.Auth;

public class AuthToken
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}