﻿namespace MagicalKitties.Application.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
    string CreateActivationToken();
    string CreateOneTimeCode();
}