namespace MagicalKitties.Application.Models.Accounts;

public class AccountActivation
{
    public required string Username { get; init; }
    public required string ActivationCode { get; init; }
    public required DateTime Expiration { get; init; }
}