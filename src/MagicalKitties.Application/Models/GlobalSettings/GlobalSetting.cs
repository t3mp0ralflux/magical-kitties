﻿namespace MagicalKitties.Application.Models.GlobalSettings;

public class GlobalSetting
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
}