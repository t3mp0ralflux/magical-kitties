using System.ComponentModel.DataAnnotations.Schema;

namespace MagicalKitties.Application.Models.Accounts;

public class Account
{
    public required Guid Id { get; init; }

    [Column("first_name")]
    public required string FirstName { get; init; }

    [Column("last_name")]
    public required string LastName { get; init; }

    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; set; }

    [Column("account_status")]
    public AccountStatus AccountStatus { get; set; }

    [Column("account_role")]
    public AccountRole AccountRole { get; init; }

    [Column("created_utc")]
    public DateTime CreatedUtc { get; set; }

    [Column("updated_utc")]
    public DateTime UpdatedUtc { get; set; }

    [Column("last_login_utc")]
    public DateTime LastLoginUtc { get; set; }

    [Column("activated_utc")]
    public DateTime? ActivatedUtc { get; set; }

    [Column("deleted_utc")]
    public DateTime? DeletedUtc { get; set; }

    [Column("activation_expiration")]
    public DateTime? ActivationExpiration { get; set; }

    [Column("activation_code")]
    public string? ActivationCode { get; set; }

    [Column("password_reset_requested_utc")]
    public DateTime? PasswordResetRequestedUtc { get; set; }

    [Column("password_reset_code")]
    public string? PasswordResetCode { get; set; }
}