﻿namespace MagicalKitties.Application.Models.Accounts;

public class Account
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    // managed by API
    public AccountStatus AccountStatus { get; set; }
    public AccountRole AccountRole { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public DateTime LastLoginUtc { get; set; }
    public DateTime? ActivatedUtc { get; set; }
    public DateTime? DeletedUtc { get; set; }
    public DateTime? ActivationExpiration { get; set; }
    public string? ActivationCode { get; set; }
    public DateTime? PasswordResetRequestedUtc { get; set; }
    public string? PasswordResetCode { get; set; }
}

public class Activation
{
    public DateTime Expiration { get; set; }
    public string Code { get; set; }

    public void WithActivation()
    {
    }
}