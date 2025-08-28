using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Models.Characters;

public class UpgradeRule
{
    public required Guid Id { get; init; }
    public required int Block { get; init; }
    public required string Value { get; init; }
    public required UpgradeOption UpgradeOption { get; set; }
}