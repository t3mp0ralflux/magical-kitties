namespace MagicalKitties.Application.Models.Auth;

public class RefreshToken
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public string AccessToken { get; set; } = "";
    public string Token { get; set; } = "";
    public DateTime ExpirationUtc { get; set; }
}