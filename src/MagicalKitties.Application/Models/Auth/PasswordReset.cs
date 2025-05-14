namespace MagicalKitties.Application.Models.Auth;

public class PasswordReset
{
    public required string Email { get; init; }
    public required string ResetCode { get; init; }
    public required string Password { get; set; }
}