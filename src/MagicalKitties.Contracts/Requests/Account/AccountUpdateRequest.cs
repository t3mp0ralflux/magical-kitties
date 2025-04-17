﻿using MagicalKitties.Contracts.Models;

namespace MagicalKitties.Contracts.Requests.Account;

public class AccountUpdateRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; set; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required AccountStatus AccountStatus { get; init; }
    public required AccountRole AccountRole { get; init; }
}