﻿namespace MagicalKitties.Contracts.Responses.Rules;

public class UpgradeRuleResponse
{
    public required Guid Id { get; init; }
    public required int Block { get; init; }
    public required string Value { get; init; }
}