﻿namespace MagicalKitties.Contracts.Requests.Account;

public class AccountCreateRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}