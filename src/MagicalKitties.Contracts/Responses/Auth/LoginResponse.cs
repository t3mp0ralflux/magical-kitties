using MagicalKitties.Contracts.Responses.Account;

namespace MagicalKitties.Contracts.Responses.Auth;

public class LoginResponse
{
    public required AccountResponse Account { get; init; }
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}